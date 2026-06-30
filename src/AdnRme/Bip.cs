#region Header
// Revit MEP API sample application
//
// Copyright (C) 2007-2021 by Jeremy Tammik, Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software
// for any purpose and without fee is hereby granted, provided
// that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  
// AUTODESK, INC. DOES NOT WARRANT THAT THE OPERATION OF THE 
// PROGRAM WILL BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject
// to restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
#endregion // Header

#region Namespaces
using Autodesk.Revit.DB;
#endregion // Namespaces

namespace AdnRme
{
  class Bip
  {
    // Short aliases for long BuiltInParameter names.
    public const BuiltInParameter Airflow = BuiltInParameter.ROOM_ACTUAL_SUPPLY_AIRFLOW_PARAM;
    public const BuiltInParameter Area = BuiltInParameter.ROOM_AREA;
    public const BuiltInParameter CalculatedSupplyAirFlow = BuiltInParameter.ROOM_CALCULATED_SUPPLY_AIRFLOW_PARAM;
    public const BuiltInParameter Flow = BuiltInParameter.RBS_DUCT_FLOW_PARAM;
    public const BuiltInParameter Host = BuiltInParameter.INSTANCE_FREE_HOST_PARAM;

    // RBS_SYSTEM_TYPE_PARAM was renamed to RBS_SYSTEM_CLASSIFICATION_PARAM in 2012.
    public const BuiltInParameter SystemType = BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM; // 2012
  }
}
