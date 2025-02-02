// Internal

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static ImportExport.SharedFunction;
using static ImportExport.Json.JsonShared;

namespace ImportExport.Json
{
    public class JsonImport
    {
        public static async void ImportJson()
        {
            string path = EditorUtility.OpenFilePanel( "Json Import", "", "json" );

            if ( path.Length == 0 ) return;

            EditorUtility.DisplayProgressBar( "Starting Import", "Reading File...", 0 );
            ReMapConsole.Log( "[Json Import] Reading file: " + path, ReMapConsole.LogType.Warning );

            string json = File.ReadAllText( path );
            jsonData = JsonUtility.FromJson< JsonData >( json );

            if ( !CheckJsonVersion( jsonData ) )
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            // Sort by alphabetical name
            foreach ( var objectType in Helper.GetAllObjectType() )
                await ExecuteJson( objectType, ExecuteType.SortList );

            foreach ( var objectType in Helper.GetAllObjectType() )
                await ExecuteJson( objectType, ExecuteType.Import );

            ReMapConsole.Log( "[Json Import] Finished", ReMapConsole.LogType.Success );

            EditorUtility.ClearProgressBar();
        }

        internal static async Task ImportObjectsWithEnum<T>( ObjectType objectType, List< T > listType ) where T : GlobalClassData
        {
            int i = 0;
            int j = 1;

            int objectsCount = listType.Count;

            foreach ( var objData in listType )
            {
                ProcessImportClassData( objData, DetermineDataName( objData, objectType ), objectType, i, j, objectsCount );

                await Helper.Wait();
                i++;
                j++;
            }
        }

        private static GameObject ProcessImportClassData<T>( T objData, string objName, ObjectType objectType, int i, int j, int objectsCount ) where T : GlobalClassData
        {
            string importing;

            if ( string.IsNullOrEmpty( objData.PathString ) )
                importing = objName;
            else importing = $"{objData.PathString}/{objName}";

            EditorUtility.DisplayProgressBar( $"Importing {Helper.GetObjNameWithEnum( objectType )} {j}/{objectsCount}", $"Importing: {importing}", ( i + 1 ) / ( float ) objectsCount );
            ReMapConsole.Log( "[Json Import] Importing: " + objName, ReMapConsole.LogType.Info );

            var obj = Helper.CreateGameObject( "", objName, PathType.Name );

            if ( !Helper.IsValid( obj ) ) return null;

            GetSetTransformData( obj, objData.TransformData );
            GetSetScriptData( objectType, obj, objData, GetSetData.Set );
            CreatePath( objData.Path, objData.PathString, obj );

            return obj;
        }

        private static string DetermineDataName<T>( T objData, ObjectType objectType ) where T : GlobalClassData
        {
            switch ( objData )
            {
                case PropClassData data: return data.Name;
                case VerticalZipLineClassData data: return data.Name;
                case NonVerticalZipLineClassData data: return data.Name;
                case WeaponRackClassData data: return data.Name;
                case BubbleShieldClassData data: return data.Name;
                case RespawnableHealClassData data: return data.Name;
            }

            return Helper.GetObjRefWithEnum( objectType );
        }

        private static bool CheckJsonVersion( JsonData jsonData )
        {
            string Version = UnityInfo.JsonVersion;
            if ( jsonData.Version != Version )
            {
                string fileVersion = jsonData.Version == null ? "Unknown Version" : jsonData.Version;

                UnityInfo.Printt( $"This json file is outdated ( {fileVersion} / {Version} )" );

                return false;
            }

            return true;
        }
    }
}