#pragma checksum "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetLocations.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1e8df2bd6926201fee97e00726cbdd776de8da5b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Pratice_GetLocations), @"mvc.1.0.view", @"/Views/Pratice/GetLocations.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\_ViewImports.cshtml"
using Urfu.Its.Frames;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\_ViewImports.cshtml"
using Urfu.Its.Frames.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1e8df2bd6926201fee97e00726cbdd776de8da5b", @"/Views/Pratice/GetLocations.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a29a642edbfb9ad6e9236f96e04f5da3c1de2bef", @"/Views/_ViewImports.cshtml")]
    public class Views_Pratice_GetLocations : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Urfu.Its.Frames.Controllers.LocationVM>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\-urfu\its\Urfu.Its.Web\Urfu.Its.Frames\Views\Pratice\GetLocations.cshtml"
Write(Html.DropDownList(Model.targetProperty, Model.Items, new { @class = "form-control col-sm-10", id = Model.listId }));

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Urfu.Its.Frames.Controllers.LocationVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
