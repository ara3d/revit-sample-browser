// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Default Workset Names
        // Shared by Julian Wandzilak in the Revit API discussion thread
        // https://forums.autodesk.com/t5/revit-api-forum/doc-enableworksharing-amp-language-versions/m-p/11845252#M70159
        /// <summary>
        /// Return default workset names 
        /// for all languages supported by Revit
        /// </summary>
        /// <param name="sLanguage">`app.Language.ToString()`</param>
        /// <returns>`false` if no valid language input argument provided, else `true`</returns>
        static bool GetDefaultWorksetNames(
            string sLanguage, 
            out string wsnLevelsAndGrids, 
            out string wsnWorkset1 )
        {
            wsnLevelsAndGrids = string.Empty;
            wsnWorkset1 = string.Empty;

            switch (sLanguage)
            {
                case "Unknown":
                    wsnLevelsAndGrids = "Shared Levels and Grids";
                    wsnWorkset1 = "Workset1";
                    break;
                case "English_USA":
                    wsnLevelsAndGrids = "Shared Levels and Grids";
                    wsnWorkset1 = "Workset1";
                    break;
                case "German":
                    wsnLevelsAndGrids = "Gemeinsam genutzte Ebenen und Raster";
                    wsnWorkset1 = "Bearbeitungsbereich1";
                    break;
                case "Spanish":
                    wsnLevelsAndGrids = "Niveles y rejillas compartidos";
                    wsnWorkset1 = "Subproyecto1";
                    break;
                case "French":
                    wsnLevelsAndGrids = "Quadrillages et niveaux partagés";
                    wsnWorkset1 = "Sous-projet 1";
                    break;
                case "Italian":
                    wsnLevelsAndGrids = "Griglie e livelli condivisi";
                    wsnWorkset1 = "Workset1";
                    break;
                //case "Dutch":
                //  wsnLevelsAndGrids = "Shared Levels and Grids";
                //  wsnWorkset1 = "Workset1";
                //  break;
                case "Chinese_Simplified":
                    wsnLevelsAndGrids = "共享标高和轴网";
                    wsnWorkset1 = "工作集1";
                    break;
                case "Chinese_Traditional":
                    wsnLevelsAndGrids = "共用的樓層和網格";
                    wsnWorkset1 = "工作集 1";
                    break;
                case "Japanese":
                    wsnLevelsAndGrids = "共有レベルと通芯";
                    wsnWorkset1 = "ワークセット1";
                    break;
                case "Korean":
                    wsnLevelsAndGrids = "공유 레벨 및 그리드";
                    wsnWorkset1 = "작업세트1";
                    break;
                case "Russian":
                    wsnLevelsAndGrids = "Общие уровни и сетки";
                    wsnWorkset1 = "Рабочий набор 1";
                    break;
                case "Czech":
                    wsnLevelsAndGrids = "Sdílená podlaží a osnovy";
                    wsnWorkset1 = "Pracovní sada1";
                    break;
                case "Polish":
                    wsnLevelsAndGrids = "Współdzielone poziomy i osie";
                    wsnWorkset1 = "Zadanie1";
                    break;
                //case "Hungarian":
                //  wsnLevelsAndGrids = "Shared Levels and Grids";
                //  wsnWorkset1 = "Workset1";
                //  break;
                case "Brazilian_Portuguese":
                    wsnLevelsAndGrids = "Níveis e eixos compartilhados";
                    wsnWorkset1 = "Workset1";
                    break;
                case "English_GB":
                    wsnLevelsAndGrids = "Shared Levels and Grids";
                    wsnWorkset1 = "Workset1";
                    break;
                default:
                    wsnLevelsAndGrids = "Shared Levels and Grids";
                    wsnWorkset1 = "Workset1";
                    break;
            }
            return 0 < wsnLevelsAndGrids.Length;
        }
        #endregion
    }
}
