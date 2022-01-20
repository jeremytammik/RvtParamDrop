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
If an instance or a type has a parameter that refers to another Element, its instance and type parameters are also exported, regardless of whether it is visible or not.
That is recursive, so if X references Y references Z references W, then W's parameters are exported if X, Y, or Z is visible.
We follow all references.
Who are we to say that a referenced `Element` isn't useful?
