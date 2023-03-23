
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static Build.Build;
using static ImportExport.Shared.SharedFunction;

namespace Build
{
    public class BuildProp
    {
        public static string BuildPropObjects( GameObject[] objectData, BuildType buildType )
        {
            string code = "";
            List< String > precacheList = new List< String >();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "    //Props";
                    code += "\n";
                    break;
                case BuildType.EntFile:
                    // Empty
                    break;
                case BuildType.Precache:
                    // Empty
                    break;
                case BuildType.DataTable:
                    code += "\"type\",\"origin\",\"angles\",\"scale\",\"fade\",\"mantle\",\"visible\",\"mdl\",\"collection\"";
                    code += "\n";
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                PropScript script = ( PropScript ) Helper.GetComponentByEnum( obj, ObjectType.Prop );
                if ( script == null ) continue;

                string model = UnityInfo.GetObjName( obj );
                string scale = obj.transform.localScale.x.ToString().Replace(",", ".");

                switch ( buildType )
                {
                    case BuildType.Script:
                        code += $"    MapEditor_CreateProp( $\"{model}\", {Helper.BuildOrigin(obj) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(obj)}, {script.AllowMantle.ToString().ToLower()}, {script.FadeDistance}, {script.RealmID}, {scale} )";
                        code += "\n";
                        break;
                    case BuildType.EntFile:
                        code +=  "{\n";
                        code +=  "\"StartDisabled\" \"0\"\n";
                        code +=  "\"spawnflags\" \"0\"\n";
                        code += $"\"fadedist\" \"{script.FadeDistance}\"\n";
                        code += $"\"collide_titan\" \"1\"\n";
                        code += $"\"collide_ai\" \"1\"\n";
                        code += $"\"scale\" \"{scale}\"\n";
                        code += $"\"angles\" \"{Helper.BuildAngles( obj, true )}\"\n";
                        code += $"\"origin\" \"{Helper.BuildOrigin( obj, true )}\"\n";
                        code +=  "\"targetname\" \"ReMapEditorProp\"\n";
                        code +=  "\"solid\" \"6\"\n";
                        code += $"\"model\" \"{model}\"\n";
                        code +=  "\"ClientSide\" \"0\"\n";
                        code +=  "\"classname\" \"prop_dynamic\"\n";
                        code +=  "}\n";
                        break;
                    case BuildType.Precache:
                        if ( precacheList.Contains( model ) )
                            continue;
                        precacheList.Add( model );
                        code += $"    PrecacheModel( $\"{model}\" )" + "\n";
                        break;
                    case BuildType.DataTable:
                        code += $"\"prop_dynamic\",\"{Helper.BuildOrigin( obj )}\",\"{Helper.BuildAngles( obj )}\",{scale},{script.FadeDistance},{script.AllowMantle.ToString().ToLower()},true,\"{UnityInfo.GetApexModelName( model )}\",\"{FindPathString( obj )}\"";
                        code += "\n";
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code += "\n";
                    break;
                case BuildType.EntFile:
                    // Empty
                    break;
                case BuildType.Precache:
                    // Empty
                    break;
                case BuildType.DataTable:
                    code += "\"string\",\"vector\",\"vector\",\"float\",\"float\",\"bool\",\"bool\",\"asset\",\"string\"";
                break;
            }

            return code;
        }
    }
}
