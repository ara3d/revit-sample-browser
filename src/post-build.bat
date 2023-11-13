set AddinsDir=%programdata%\Autodesk\Revit\Addins
xcopy /Y *.addin %AddinsDir%\2024
mkdir %AddinsDir%\2024\MultiSample
xcopy /Y %1 %AddinsDir%\2024\MultiSample