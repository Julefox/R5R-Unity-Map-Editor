
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static Build.Build;

namespace Build
{
    public class BuildNonVerticalZipline
    {
        public static async Task< StringBuilder > BuildNonVerticalZipLineObjects( GameObject[] objectData, BuildType buildType )
        {
            StringBuilder code = new StringBuilder();

            // Add something at the start of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    code.Append( "    // NonVerticalZipLines" );
                    PageBreak( ref code );
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
                DrawNonVerticalZipline script = ( DrawNonVerticalZipline ) Helper.GetComponentByEnum( obj, ObjectType.NonVerticalZipLine );
                if ( script == null ) continue;

                int PreserveVelocity = script.PreserveVelocity  ? 1 : 0;
                int DropToBottom = script.DropToBottom ? 1 : 0;
                string RestPoint = script.RestPoint.ToString().ToLower();
                int PushOffInDirectionX = script.PushOffInDirectionX ? 1 : 0;
                string IsMoving = script.IsMoving.ToString().ToLower();
                int DetachEndOnSpawn = script.DetachEndOnSpawn ? 1 : 0;
                int DetachEndOnUse = script.DetachEndOnUse ? 1 : 0;
                float PanelTimerMin = script.PanelTimerMin;
                float PanelTimerMax = script.PanelTimerMax;
                int PanelMaxUse = script.PanelMaxUse;

                string PanelOrigin = BuildVerticalZipline.BuildPanelOriginArray( script.Panels );
                string PanelAngles = BuildVerticalZipline.BuildPanelAnglesArray( script.Panels );
                string PanelModels = BuildVerticalZipline.BuildPanelModelsArray( script.Panels );
                string LinkGuid = Helper.GetRandomGUIDForEnt();
                string LinkGuidTo0 = Helper.GetRandomGUIDForEnt();

                switch ( buildType )
                {
                    case BuildType.Script:
                        code.Append( $"    MapEditor_CreateZiplineFromUnity( {Helper.BuildOrigin(script.rope_start.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildOrigin(script.rope_end.gameObject) + Helper.ShouldAddStartingOrg()}, {Helper.BuildAngles(script.rope_start.gameObject)}, false, {Helper.ReplaceComma( script.FadeDistance )}, {Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )}, {PreserveVelocity}, {DropToBottom}, {Helper.ReplaceComma( script.AutoDetachStart )}, {Helper.ReplaceComma( script.AutoDetachEnd )}, {RestPoint}, {PushOffInDirectionX}, {IsMoving}, {DetachEndOnSpawn}, {DetachEndOnUse}, {PanelOrigin}, {PanelAngles}, {PanelModels}, {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )" );
                        PageBreak( ref code );
                        break;

                    case BuildType.EntFile:
                        code.Append(  "{" ); PageBreak( ref code );
                        code.Append( $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\"" ); PageBreak( ref code );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true, true )}\"" ); PageBreak( ref code );
                        code.Append( $"\"link_guid\" \"{LinkGuid}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachEnd )}\"" ); PageBreak( ref code );
                        code.Append( $"\"classname\" \"zipline_end\"" ); PageBreak( ref code );
                        code.Append(  "}" ); PageBreak( ref code );
                        code.Append(  "{" ); PageBreak( ref code );

                        if ( script.RestPoint )
                        {
                            code.Append( $"\"_zipline_rest_point_1\" \"{Helper.BuildOrigin( script.rope_end.gameObject, true )}\"" ); PageBreak( ref code );
                            code.Append( $"\"_zipline_rest_point_0\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true )}\"" ); PageBreak( ref code );
                        }

                        code.Append( $"\"ZiplinePreserveVelocity\" \"{PreserveVelocity}\"" );PageBreak( ref code );
                        code.Append( $"\"ZiplineFadeDistance\" \"{Helper.ReplaceComma( script.FadeDistance )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineDropToBottom\" \"{DropToBottom}\"" ); PageBreak( ref code );
                        code.Append( $"\"Width\" \"{Helper.ReplaceComma( script.Width )}\"" ); PageBreak( ref code );
                        code.Append( $"\"Material\" \"cable/zipline.vmt\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_freedm\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_control\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_arenas\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"DetachEndOnUse\" \"{DetachEndOnUse}\"" ); PageBreak( ref code );
                        code.Append( $"\"DetachEndOnSpawn\" \"{DetachEndOnSpawn}\"" ); PageBreak( ref code );
                        code.Append( $"\"scale\" \"{Helper.ReplaceComma( script.Scale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"angles\" \"{Helper.BuildAngles( script.rope_start.gameObject, true )}\"" ); PageBreak( ref code );
                        code.Append( $"\"origin\" \"{Helper.BuildOrigin( script.rope_start.gameObject, true, true )}\"" ); PageBreak( ref code );
                        code.Append( $"\"link_to_guid_0\" \"{LinkGuidTo0}\"" ); PageBreak( ref code );
                        code.Append( $"\"link_guid\" \"{LinkGuid}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineVertical\" \"0\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineVersion\" \"3\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineSpeedScale\" \"{Helper.ReplaceComma( script.SpeedScale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplinePushOffInDirectionX\" \"{PushOffInDirectionX}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineLengthScale\" \"{Helper.ReplaceComma( script.LengthScale )}\"" ); PageBreak( ref code );
                        code.Append( $"\"ZiplineAutoDetachDistance\" \"{Helper.ReplaceComma( script.AutoDetachStart )}\"" ); PageBreak( ref code );
                        code.Append( $"\"gamemode_survival\" \"1\"" ); PageBreak( ref code );
                        code.Append( $"\"classname\" \"zipline\"" ); PageBreak( ref code );
                        code.Append(  "}" ); PageBreak( ref code );
                        break;

                    case BuildType.Precache:
                        // Empty
                        break;

                    case BuildType.DataTable:
                        // Empty
                        break;

                    case BuildType.LiveMap:
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetOrigin( {Helper.BuildOrigin(script.rope_start.gameObject, false, true)}, {Helper.BuildOrigin(script.rope_end.gameObject, false, true)} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetAngles( {Helper.BuildAngles(script.rope_start.gameObject)}, {Helper.BuildAngles(script.rope_start.gameObject)} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetZiplineVars01( false, {Helper.ReplaceComma( script.FadeDistance )}, {Helper.ReplaceComma( script.Scale )}, {Helper.ReplaceComma( script.Width )}, {Helper.ReplaceComma( script.SpeedScale )}, {Helper.ReplaceComma( script.LengthScale )}, {PreserveVelocity}, {DropToBottom} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetZiplineVars02( {Helper.ReplaceComma( script.AutoDetachStart )}, {Helper.ReplaceComma( script.AutoDetachEnd )}, {Helper.BoolToLower( script.RestPoint )}, {PushOffInDirectionX}, {Helper.BoolToLower( script.IsMoving )}, {DetachEndOnSpawn}, {DetachEndOnUse} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetRemapArrayVec01( {PanelOrigin} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetRemapArrayVec02( {PanelAngles} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetZiplinePanelModel( {PanelModels} )" );
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapSetZiplinePanelSettings( {PanelTimerMin}, {PanelTimerMax}, {PanelMaxUse} )");
                        CodeViewsWindow.LiveMap.AddToGameQueue( $"script ReMapCreateZipline()" );
                    break;
                }
            }

            // Add something at the end of the text
            switch ( buildType )
            {
                case BuildType.Script:
                    PageBreak( ref code );
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
    }
}
