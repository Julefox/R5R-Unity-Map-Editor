
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildVerticalZipline
    {
        public static async Task< StringBuilder > BuildVerticalZipLineObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    AppendCode( ref code, "    // VerticalZipLines" );
                    break;
                    
                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    // Empty
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            // Build the code
            foreach ( GameObject obj in objectData )
            {
                DrawVerticalZipline script = ( DrawVerticalZipline ) Helper.GetComponentByEnum( obj, ObjectType.VerticalZipLine );
                if ( script == null ) continue;

                int PreserveVelocity = script.PreserveVelocity  ? 1 : 0;
                int DropToBottom = script.DropToBottom ? 1 : 0;
                int PushOffInDirectionX = script.PushOffInDirectionX ? 1 : 0;
                int DetachEndOnSpawn = script.DetachEndOnSpawn ? 1 : 0;
                int DetachEndOnUse = script.DetachEndOnUse ? 1 : 0;
                string PanelTimerMin = Helper.ReplaceComma( script.PanelTimerMin );
                string PanelTimerMax = Helper.ReplaceComma( script.PanelTimerMax );
                int PanelMaxUse = script.PanelMaxUse;

                string PanelOrigin = BuildPanelOriginArray( script.Panels );
                string PanelAngles = BuildPanelAnglesArray( script.Panels );
                string PanelModels = BuildPanelModelsArray( script.Panels );
                string LinkGuid = Helper.GetRandomGUIDForEnt();
                string LinkGuidTo0 = Helper.GetRandomGUIDForEnt();

                switch ( buildType )
                {
                    case BuildType.Script:
                        AppendCode( ref code, $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(script.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildOrigin(script.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, true, {Helper.ReplaceComma( script.FadeDistance )}, {Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )}, {PreserveVelocity}, {DropToBottom}, {Helper.ReplaceComma( script.AutoDetachStart )}, {Helper.ReplaceComma( script.AutoDetachEnd )}, {Helper.BoolToLower( script.RestPoint )}, {PushOffInDirectionX}, {Helper.BoolToLower( script.IsMoving )}, {DetachEndOnSpawn}, {DetachEndOnUse}, {PanelOrigin}, {PanelAngles}, {PanelModels}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )" );
                        break;

                    case BuildType.EntFile:
                        AppendCode( ref code,  "{" );
                        AppendCode( ref code, $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true, true )}\"" );
                        AppendCode( ref code, $"\"link_guid\" \"{LinkGuid}\"" );
                        AppendCode( ref code, $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\"" );
                        AppendCode( ref code, $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachEnd )}\"" );
                        AppendCode( ref code, $"\"classname\" \"zipline_end\"" );
                        AppendCode( ref code,  "}" );
                        AppendCode( ref code,  "{" );

                        if ( script.RestPoint )
                        {
                            AppendCode( ref code, $"\"_zipline_rest_point_1\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true )}\"" );
                            AppendCode( ref code, $"\"_zipline_rest_point_0\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true )}\"" );
                        }

                        AppendCode( ref code, $"\"ZiplinePreserveVelocity\" \"{PreserveVelocity}\"" );
                        AppendCode( ref code, $"\"ZiplineFadeDistance\" \"{Helper.ReplaceComma( script.FadeDistance )}\"" );
                        AppendCode( ref code, $"\"ZiplineDropToBottom\" \"{DropToBottom}\"" );
                        AppendCode( ref code, $"\"Width\" \"{Helper.ReplaceComma( script.Width )}\"" );
                        AppendCode( ref code, $"\"Material\" \"cable/zipline.vmt\"" );
                        AppendCode( ref code, $"\"gamemode_freedm\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_control\" \"1\"" );
                        AppendCode( ref code, $"\"gamemode_arenas\" \"1\"" );
                        AppendCode( ref code, $"\"DetachEndOnUse\" \"{DetachEndOnUse}\"" );
                        AppendCode( ref code, $"\"DetachEndOnSpawn\" \"{DetachEndOnSpawn}\"" );
                        AppendCode( ref code, $"\"scale\" \"{Helper.ReplaceComma( script.Scale )}\"" );
                        AppendCode( ref code, $"\"angles\" \"{Helper.BuildAngles( script.rope_start.gameObject, true )}\"" );
                        AppendCode( ref code, $"\"origin\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true, true )}\"" );
                        AppendCode( ref code, $"\"link_to_guid_0\" \"{LinkGuidTo0}\"" );
                        AppendCode( ref code, $"\"link_guid\" \"{LinkGuid}\"" );
                        AppendCode( ref code, $"\"ZiplineVertical\" \"1\"" );
                        AppendCode( ref code, $"\"ZiplineVersion\" \"3\"" );
                        AppendCode( ref code, $"\"ZiplineSpeedScale\" \"{Helper.ReplaceComma( script.SpeedScale )}\"" );
                        AppendCode( ref code, $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\"" );
                        AppendCode( ref code, $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\"" );
                        AppendCode( ref code, $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachStart )}\"" );
                        AppendCode( ref code, $"\"gamemode_survival\" \"1\"" );
                        AppendCode( ref code, $"\"classname\" \"zipline\"" );
                        AppendCode( ref code,  "}" );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetOrigin({Helper.BuildOrigin(script.rope_start.gameObject, false, true)}, {Helper.BuildOrigin(script.rope_end.gameObject, false, true)})" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetAngles({Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildAngles(script.rope_start.gameObject)})" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetBool([true,{Helper.BoolToLower( script.RestPoint )},{Helper.BoolToLower( script.IsMoving )}])" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetFloat([{Helper.ReplaceComma( script.FadeDistance, true )},{Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )},{Helper.ReplaceComma( script.AutoDetachStart )},{Helper.ReplaceComma( script.AutoDetachEnd )},{PanelTimerMin}, {PanelTimerMax}])" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetInt([{PreserveVelocity},{DropToBottom},{PushOffInDirectionX},{DetachEndOnSpawn},{DetachEndOnUse},{PanelMaxUse}])" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetVectorArray01({PanelOrigin})" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetVectorArray02({PanelAngles})" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetAssetArray01({PanelModels})" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapCreateZipline()" );
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                   
                    break;

                case BuildType.EntFile:
                    // Empty
                    break;

                case BuildType.Precache:
                    // Empty
                    break;

                case BuildType.DataTable:
                    // Empty
                    break;

                case BuildType.LiveMap:
                    // Empty
                break;
            }

            await Task.Delay( TimeSpan.FromSeconds( 0.001 ) );

            return code;
        }

        internal static string BuildPanelOriginArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $" {Helper.BuildOrigin( objArray[i], false, true )}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        internal static string BuildPanelAnglesArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"{Helper.BuildAngles( objArray[i] )}";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }

        internal static string BuildPanelModelsArray( GameObject[] objArray )
        {
            string array = "[ ";
            for( int i = 0 ; i < objArray.Length ; i++ )
            {
                array += $"$\"{UnityInfo.GetApexModelName( "mdl/" + objArray[i].name, true )}\"";

                if ( i != objArray.Length - 1 ) array += ", ";
            }
            array += " ]";

            return array;
        }
    }
}
