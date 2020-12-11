function checkchange_base (node, checked) {
                                node.cascadeBy(function (child) {

                                    if (checked) {
                                        while (child.parentNode !== null) {
                                            child.set('checked', checked);
                                            child = child.parentNode;
                                        }
                                    }
                                    else {
                                        child.set('checked', checked);
                                        node.cascadeBy(function (child) {
                                            child.set('checked', checked);
                                        });
                                        var currentNode = child;
                                        while (currentNode.parentNode !== null) {
                                            currentNode = currentNode.parentNode;
                                            if (!currentNode.data.children) {
                                                break;
                                            }
                                            var r = currentNode.data.children.some(function (child) {
                                                return child.checked;
                                            });
                                            if (!r) {
                                                currentNode.set('checked', checked);
                                            }
                                            else {
                                                break;
                                            }
                                        }
                                    }

                                });
                            };
  