using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Urfu.Its.VersionedDocs.Services
{
    public class JSchemaObjectActivator
    {        
        public object Create(string json, JSchema schema)
        {
            var type = CreateType(schema);
            var destObject = JsonConvert.DeserializeObject(json, type);
            return destObject;
        }

        public Type CreateType(JSchema schema)
        {
            if (!schema.Properties.Any())
            {
                if (schema.Type.GetValueOrDefault().HasFlag(JSchemaType.Array))
                {
                    var itemSchema = schema.Items.First();
                    var type = CreateType(itemSchema);
                    var listType = typeof(List<>).MakeGenericType(type);
                    return listType;
                }

                var t = ConvertType(schema.Type.Value);
                return t;
            }
            var typeBuilder = GetTypeBuilder(Guid.NewGuid().ToString());
            foreach (var item in schema.Properties)
            {
                if (item.Value.Type.GetValueOrDefault().HasFlag(JSchemaType.Object))
                {
                    var type = CreateType(item.Value);
                    if (item.Value.Type != null)
                    {
                        CreateProperty(typeBuilder, item.Key, type);
                    }
                }
                else if (item.Value.Type.GetValueOrDefault().HasFlag(JSchemaType.Array))
                {
                    var itemSchema = item.Value.Items.First();
                    var type = CreateType(itemSchema);
                    var listType = typeof(List<>).MakeGenericType(type);
                    if (itemSchema.Type != null)
                    {
                        CreateProperty(typeBuilder, item.Key, listType);
                    }
                }
                else
                {
                    if (item.Value.Type != null)
                    {
                        CreateProperty(typeBuilder, item.Key, ConvertType(item.Value.Type.Value));
                    }
                }
            }

            var result = typeBuilder.CreateType();
            return result;
        }

        private static Type ConvertType(JSchemaType source)
        {         
            switch (source)
            {
                case JSchemaType.None:

                    break;
                case JSchemaType.String:
                case JSchemaType.String | JSchemaType.Null:
                    return typeof(string);                                    
                case JSchemaType.Number: 
                    return typeof(decimal);                    
                case JSchemaType.Number | JSchemaType.Null:
                    return typeof(decimal?);                    
                case JSchemaType.Integer:
                    return typeof(int);                    
                case JSchemaType.Integer | JSchemaType.Null:
                    return typeof(int?);                    
                case JSchemaType.Boolean:
                    return typeof(bool);                    
                case JSchemaType.Boolean | JSchemaType.Null:
                    return typeof(bool?);                    
                case JSchemaType.Object:
                case JSchemaType.Object | JSchemaType.Null:
                    return typeof(object);                
            }
            throw new NotSupportedException($"Тип '{source}' не поддерживается");
        }

        private static TypeBuilder GetTypeBuilder(string typeSignature)
        {
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }        
    }
}