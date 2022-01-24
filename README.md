# RvtParamDrop

Revit .NET C# add-in to export parameter values for elements visible in current view

Generate a count and a `csv` of Revit parameters on elements in a view.

- Do not limit yourself to shared parameters
- Do limit yourself to parameters with a value
- Name of the parameter
- Schema (`TypeId`)

A small number of parameters are intentionally ignored as redundant:

- ELEM_CATEGORY_PARAM
- ELEM_CATEGORY_PARAM_MT
- ELEM_FAMILY_AND_TYPE_PARAM
- ELEM_TYPE_PARAM
- SYMBOL_ID_PARAM

Include parameters from both elements and their types, i.e., both instance and type parameters.

Actually, it's more complicated than that.
Anything that is visible in the view will include its instance and type parameters.
If an instance or a type has a parameter that refers to another `Element`, its instance and type parameters are also exported, regardless of whether it is visible or not.
That is recursive, so if X references Y references Z references W, then W's parameters are exported if X, Y, or Z is visible.
We follow all references.
Who are we to say that a referenced `Element` isn't useful?

To be verified:

- All elements of interest and all their references are included
- All parameter values of interest were included
- For each parameter value, all required data fields were included

The views to be processed are 3D views.

## Installation

Standard Revit add-in installation, cf.
[Revit Developer's Guide](https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_html)
[Add-in Registration](https://help.autodesk.com/view/RVT/2022/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Introduction_Add_In_Integration_Add_in_Registration_html):

- Copy the add-in manifest `*.addin` and .NET assembly `DLL` into the Revit `AddIns` folder and restart Revit
- Click the menu entry under `External Tools` &rarr; `RvtParamDrop`

## Todo

- Define precision for floating point numbers.

## Author

Jeremy Tammik,
[The Building Coder](http://thebuildingcoder.typepad.com) and
[The 3D Web Coder](http://the3dwebcoder.typepad.com),
[Forge](http://forge.autodesk.com) [Platform](https://developer.autodesk.com) Development,
[ADN](http://www.autodesk.com/adn)
[Open](http://www.autodesk.com/adnopen),
[Autodesk Inc.](http://www.autodesk.com)

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
Please see the [LICENSE](LICENSE) file for full details.
