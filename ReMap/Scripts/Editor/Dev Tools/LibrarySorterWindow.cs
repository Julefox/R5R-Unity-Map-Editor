
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace LibrarySorter
{
    internal enum TaskType
    {
        FixPrefabsTags,
        FixPrefabsLabels,
        DeleteUnusuedTextures,
        CheckTextures,
        FixLodsScale,
        CreateRpakList,
        FixPrefabsData,
        FixAllPrefabsData,
        FixSpecificPrefabData
    }

    public class LibrarySorterWindow : EditorWindow
    {
        internal static LibraryData libraryData;
        internal static RpakData all_model;
        internal static bool checkExist = false;
        Vector2 scrollPos = Vector2.zero;
        Vector2 scrollPosSearchPrefabs = Vector2.zero;

        static bool foldoutFixFolders = true;
        static bool foldoutSearchPrefab = true;
        static string searchEntry = "";
        static string search = "";
        static List< string > searchResult = new List< string >();

        static string relativeLegionPlus = $"Assets/ReMap/LegionPlus";
        static string relativeLegionPlusExportedFiles = $"Assets/ReMap/LegionPlus/exported_files";
        static string relativeLegionExecutive = $"{UnityInfo.currentDirectoryPath}/{relativeLegionPlus}/LegionPlus.exe";

        static Process Legion = null;
        
        public static void Init()
        {
            libraryData = RpakManagerWindow.FindLibraryDataFile();

            LibrarySorterWindow window = ( LibrarySorterWindow )GetWindow( typeof( LibrarySorterWindow ), false, "Prefab Fix Manager" );
            window.minSize = new Vector2( 650, 600 );
            window.Show();
        }

        private void OnEnable()
        {
            libraryData = RpakManagerWindow.FindLibraryDataFile();
            all_model = RpakManagerWindow.FindAllModel();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical( "box" );

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Rpak Manager Window", "", () => RpakManagerWindow.Init() );
                    WindowUtility.WindowUtility.CreateButton( "Offset Manager Window", "", () => OffsetManagerWindow.Init() );

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                    WindowUtility.WindowUtility.CreateButton( "Check Prefabs Tags", "", () => AwaitTask( TaskType.FixPrefabsTags ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Prefabs Labels", "", () => AwaitTask( TaskType.FixPrefabsLabels ) );
                    WindowUtility.WindowUtility.CreateButton( "Unusued Textures", "", () => AwaitTask( TaskType.DeleteUnusuedTextures ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Textures", "", () => AwaitTask( TaskType.CheckTextures ) );
                    WindowUtility.WindowUtility.CreateButton( "Check Lods Scale", "", () => AwaitTask( TaskType.FixLodsScale ) );
                    WindowUtility.WindowUtility.CreateButton( "Create Rpak List", "", () => AwaitTask( TaskType.CreateRpakList ) );

                GUILayout.EndHorizontal();

                GUILayout.Space( 4 );

                scrollPos = EditorGUILayout.BeginScrollView( scrollPos );
                    foldoutFixFolders = EditorGUILayout.BeginFoldoutHeaderGroup( foldoutFixFolders, "Fix Folders" );
                    if ( foldoutFixFolders )
                    {
                        foreach ( RpakData data in libraryData.RpakList )
                        {
                            GUILayout.BeginHorizontal();
                                if ( WindowUtility.WindowUtility.CreateButton( $"{data.Name}", "", () => AwaitTask( TaskType.FixPrefabsData, null, data ), 260 ) )
                                {
                                    GUILayout.EndHorizontal();
                                    EditorGUILayout.EndFoldoutHeaderGroup();
                                    GUILayout.EndScrollView();
                                    GUILayout.EndVertical();
                                    return;
                                }
                                WindowUtility.WindowUtility.CreateButton( $"Find Missing", "TODO", 160 );
                                WindowUtility.WindowUtility.CreateTextInfo( $"Lastest Check: {data.Update}", "" );
                            GUILayout.EndHorizontal();
                        }
                        WindowUtility.WindowUtility.CreateButton( $"Check All", "", () => AwaitTask( TaskType.FixAllPrefabsData ) );
                        GUILayout.Space( 4 );
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    foldoutSearchPrefab = EditorGUILayout.BeginFoldoutHeaderGroup( foldoutSearchPrefab, "Fix Prefabs" );
                    if ( foldoutSearchPrefab )
                    {
                        searchEntry = EditorGUILayout.TextField( searchEntry );

                        if ( searchEntry.Length >= 3 )
                        {
                            if ( searchEntry != search )
                            {
                                search = searchEntry;
                                SearchPrefabs( search );
                            }
                        }

                        scrollPosSearchPrefabs = EditorGUILayout.BeginScrollView( scrollPosSearchPrefabs );

                            GUILayout.Space( 10 );
                            if ( searchEntry.Length >= 3 )
                            {
                                if ( searchResult.Count != 0 )
                                {
                                    foreach ( string prefab in searchResult )
                                    {
                                        string prefabName = UnityInfo.GetUnityModelName( prefab );
                                        WindowUtility.WindowUtility.CreateButton( $"{prefabName}", "", () => AwaitTask( TaskType.FixSpecificPrefabData, prefabName ) );
                                    }
                                }
                                else WindowUtility.WindowUtility.CreateTextInfoCentered( "No results.", "" );
                            }
                            else if ( searchEntry.Length != 0 )
                            {
                                WindowUtility.WindowUtility.CreateTextInfoCentered( "Search must be at least 3 characters long.", "" );
                            }

                        GUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        internal static async void AwaitTask( TaskType taskType, string arg = null, RpakData data = null )
        {
            switch ( taskType )
            {
                case TaskType.FixPrefabsTags:
                    if ( !DoStartTask() ) return;
                    await FixPrefabsTags();
                    break;

                case TaskType.FixPrefabsLabels:
                    if ( !DoStartTask() ) return;
                    await SetModelLabels( arg );
                    break;

                case TaskType.DeleteUnusuedTextures:
                    if ( !DoStartTask() ) return;
                    await DeleteNotUsedTexture();
                    break;

                case TaskType.CheckTextures:
                    if ( !DoStartTask() ) return;
                    await CheckTexture();
                    break;

                case TaskType.FixLodsScale:
                    if ( !DoStartTask() ) return;
                    await SetScale100ToFBX();
                    break;

                case TaskType.CreateRpakList:
                    if ( !DoStartTask() ) return;
                    await CreateRpakList();
                    break;

                case TaskType.FixPrefabsData:
                    if ( !DoStartTask() ) return;
                    CheckExisting();
                    await SortFolder( data );
                    await SetModelLabels( data.Name );
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixAllPrefabsData:
                    if ( !DoStartTask() ) return;
                    CheckExisting();
                    foreach ( RpakData _data in libraryData.RpakList )
                    {
                        await SortFolder( _data );
                        await SetModelLabels( _data.Name );
                    }
                    RpakManagerWindow.SaveJson();
                    break;

                case TaskType.FixSpecificPrefabData:
                    checkExist = true;
                    await FixPrefab( arg );
                    await SetModelLabels( arg );
                break;
            }
        }

        internal static async Task SortFolder( RpakData data )
        {
            string rpakPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}";

            if ( !Directory.Exists( rpakPath ) )
            {
                ReMapConsole.Log( $"[Library Sorter] Creating directory: {UnityInfo.relativePathPrefabs}/{data.Name}", ReMapConsole.LogType.Info );
                Directory.CreateDirectory( rpakPath );
            }

            GameObject parent; GameObject obj; string modelName;

            float progress = 0.0f; int min = 0; int max = data.Data.Count;

            List< string > modelImporter = new List< string >();

            foreach ( string model in data.Data )
            {
                modelName = Path.GetFileNameWithoutExtension( model );

                string filePath = $"{relativeLegionPlusExportedFiles}/models/{modelName}/{modelName}_LOD0.fbx";
                string matsPath = $"{relativeLegionPlusExportedFiles}/models/{modelName}/_images";
                string gotoPathModel = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx";
                string gotoPathMaterial = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}";

                // If LOD0 doesn't exist
                if ( !Helper.LOD0_Exist( modelName ) )
                {
                    GUIUtility.systemCopyBuffer = model;

                    if ( !Helper.IsValid( Legion ) ) OpenLegion();

                    while ( !File.Exists( filePath ) )
                    {
                        await Helper.Wait( 1.0 );

                        EditorUtility.DisplayProgressBar( $"Sorting Folder Paused ({min}/{max})", $"\"{modelName}\" need to be imported.", progress );

                        if ( !Helper.IsValid( Legion ) ) break;
                    }

                    File.Move( filePath, gotoPathModel );
                    if ( File.Exists( $"{filePath}.meta" ) ) File.Move( $"{filePath}.meta", $"{gotoPathModel}.meta" );

                    foreach ( string matsFile in Directory.GetFiles( matsPath ) )
                    {
                        string fileName = Path.GetFileName( matsFile );

                        if ( fileName.Contains( "_albedoTexture" ) )
                        {
                            if ( !File.Exists( $"{gotoPathMaterial}/{fileName}" ) )
                            {
                                File.Move( matsFile, $"{gotoPathMaterial}/{fileName}" );
                                if ( File.Exists( $"{matsFile}.meta" ) ) File.Move( $"{matsFile}.meta", $"{fileName}.meta" );
                            }
                        }
                        else
                        {
                            File.Delete( matsFile );
                            if ( File.Exists( $"{matsFile}.meta" ) ) File.Delete( $"{matsFile}.meta" );
                        }
                    }

                    string dir = $"{relativeLegionPlusExportedFiles}/models/{modelName}";
                    if ( Directory.Exists( dir ) )
                    {
                        Directory.Delete( dir, true );

                        if ( File.Exists( $"{dir}.meta" ) )
                        {
                            File.Delete( $"{dir}.meta" );
                        }
                    }

                    modelImporter.Add( $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx" );
                }

                string unityName = UnityInfo.GetUnityModelName( model );

                if ( !File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathModel}/{data.Name}/{unityName}.prefab" ) )
                {
                    parent = Helper.CreateGameObject( unityName );
                    obj = Helper.CreateGameObject( "", $"{UnityInfo.relativePathModel}/{modelName}_LOD0.fbx", parent );

                    if ( !Helper.IsValid( parent ) || !Helper.IsValid( obj ) )
                        continue;

                    parent.AddComponent< PropScript >();
                    parent.transform.position = Vector3.zero;
                    parent.transform.eulerAngles = Vector3.zero;

                    obj.transform.position = Vector3.zero;
                    obj.transform.eulerAngles = FindAnglesOffset( model );
                    obj.transform.localScale = new Vector3(1, 1, 1);

                    parent.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                    CheckBoxColliderComponent( parent );

                    //AssetDatabase.SetLabels( ( UnityEngine.Object ) parent, new[] { model.Split( '/' )[1] } );

                    PrefabUtility.SaveAsPrefabAsset( parent, $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}.prefab" );

                    UnityEngine.Object.DestroyImmediate( parent );

                    ReMapConsole.Log( $"[Library Sorter] Created and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}", ReMapConsole.LogType.Info ); 
                }
                else if ( checkExist )
                {
                    parent = Helper.CreateGameObject( $"{UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}.prefab" );
                    obj = parent.GetComponentsInChildren< Transform >()[1].gameObject;
                    
                    if ( !Helper.IsValid( parent ) || !Helper.IsValid( obj ) )
                        continue;

                    parent.transform.position = Vector3.zero;
                    parent.transform.eulerAngles = Vector3.zero;
                    obj.transform.eulerAngles = FindAnglesOffset( model );
                    obj.transform.position = Vector3.zero;

                    CheckBoxColliderComponent( parent );

                    //AssetDatabase.SetLabels( ( UnityEngine.Object ) parent, new[] { model.Split( '/' )[1] } );

                    PrefabUtility.SavePrefabAsset( parent );

                    ReMapConsole.Log( $"[Library Sorter] Fixed and saved prefab: {UnityInfo.relativePathPrefabs}/{data.Name}/{unityName}", ReMapConsole.LogType.Success );
                }

                // Update progress bar
                progress += 1.0f / max;
                EditorUtility.DisplayProgressBar( $"Sorting Folder ({min++}/{max})", $"Processing... {modelName}", progress );
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            min = 0; max = modelImporter.Count; progress = 0.0f;
            foreach ( string modelPath in modelImporter )
            {
                ModelImporter importer = AssetImporter.GetAtPath( modelPath ) as ModelImporter;
                if ( Helper.IsValid( importer ) && importer.globalScale != 100 )
                {
                    importer.globalScale = 100;
                    importer.SaveAndReimport();
                }
                
                progress += 1.0f / max;
                EditorUtility.DisplayProgressBar( $"ReImport LOD0", $"Processing... ({min++}/{max})", progress );
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            data.Update = DateTime.UtcNow.ToString();

            return;
        }

        private static async void OpenLegion()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = relativeLegionExecutive;
            startInfo.UseShellExecute = false;

            using ( Process process = new Process() )
            {
                process.StartInfo = startInfo;
                process.Start();

                Legion = process;

                await Task.Run( () => Legion.WaitForExit() );

                Legion = null;
            }
        }

        public static Vector3 FindAnglesOffset( string searchTerm )
        {
            Vector3 returnedVector = new Vector3( 0, -90, 0 );

            PrefabOffset offset = FindPrefabOffsetFile().Find( o => o.ModelName == searchTerm );
            if ( offset != null )
            {
                returnedVector = offset.Rotation;
                ReMapConsole.Log( $"[Library Sorter] Angle override found for {searchTerm}, setting angles to: {returnedVector}", ReMapConsole.LogType.Info );
            }

            return returnedVector;
        }

        private static void CheckBoxColliderComponent( GameObject go )
        {
            BoxCollider collider = go.GetComponent< BoxCollider >();

            if ( collider == null )
            {
                go.AddComponent< BoxCollider >();
                collider = go.GetComponent< BoxCollider >();
            }

            float x = 0, y = 0, z = 0;

            foreach( Renderer o in go.GetComponentsInChildren< Renderer >() )
            {
                if( o.bounds.size.x > x ) x = o.bounds.size.x;

                if( o.bounds.size.y > y ) y = o.bounds.size.y;

                if( o.bounds.size.z > z ) z = o.bounds.size.z;
            }

            collider.size = new Vector3( x, y, z );
        }

        internal static Task SetModelLabels( string specificModelOrFolderOrnull = null )
        {
            string specificFolder = $"";
            string specificModel = $"mdl#";

            if ( specificModelOrFolderOrnull != null )
            {
                if ( specificModelOrFolderOrnull.Contains("mdl#") )
                {
                    specificModel = specificModelOrFolderOrnull;
                }
                else specificFolder = $"/{specificModelOrFolderOrnull}";
            }

            string[] guids = AssetDatabase.FindAssets( $"{specificModel}", new [] {$"{UnityInfo.relativePathPrefabs}{specificFolder}"} );
            int i = 0; int total = guids.Length;
            foreach ( var guid in guids )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath( assetPath );
                string category = assetPath.Split( "#" )[1].Replace( "Assets/Prefabs/", "" ).Replace( ".prefab", "" ).ToLower();

                string[] modelnamesplit = assetPath.Split( "/" );
                string modelname = modelnamesplit[modelnamesplit.Length - 1].Replace( ".prefab", "" );

                string[] labels = AssetDatabase.GetLabels( asset );

                if( !labels.Contains( category ) )
                {
                    AssetDatabase.SetLabels(asset, new []{category});
                    ReMapConsole.Log( $"[Library Sorter] Setting label for {modelname} to {category}", ReMapConsole.LogType.Info );
                }

                EditorUtility.DisplayProgressBar( $"Sorting Tags {i}/{total}", $"Setting {modelname} to {category}", ( i + 1 ) / ( float )total ); i++;
            }

            ReMapConsole.Log($"[Library Sorter] Finished setting labels", ReMapConsole.LogType.Success);
            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static Task FixPrefabsTags()
        {
            string[] prefabs = AssetDatabase.FindAssets("t:prefab", new[] { UnityInfo.relativePathPrefabs });

            int i = 0; int total = prefabs.Length;
            foreach ( string prefab in prefabs )
            {
                string path = AssetDatabase.GUIDToAssetPath( prefab );
                string pathReplace = path.Replace( "Assets/Prefabs/", "" );

                if ( path.Contains( "_custom_prefabs" ) )
                {
                    i++;
                    continue;
                }

                UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( $"{path}", typeof( UnityEngine.Object ) ) as GameObject;
                if ( loadedPrefabResource == null )
                {
                    ReMapConsole.Log( $"[Library Sorter] Error loading prefab: {path}", ReMapConsole.LogType.Error );
                    i++;
                    continue;
                }

                EditorUtility.DisplayProgressBar( $"Fixing Prefabs Tags {i}/{total}", $"Checking: {path}", ( i + 1 ) / ( float )total );

                if( loadedPrefabResource.tag != Helper.GetObjTagNameWithEnum( ObjectType.Prop ) )
                    loadedPrefabResource.tag = Helper.GetObjTagNameWithEnum( ObjectType.Prop );

                ReMapConsole.Log( $"[Library Sorter] Set {path} tag to: {Helper.GetObjTagNameWithEnum( ObjectType.Prop )}", ReMapConsole.LogType.Info );

                PrefabUtility.SavePrefabAsset( loadedPrefabResource ); i++;
            }

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static async Task CheckTexture()
        {
            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel } );

            float progress = 0.0f; int min = 0; int max = modeltextureGUID.Length;

            List< string > textures = new List< string >();

            foreach ( var guid in modeltextureGUID )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                GameObject obj = Helper.CreateGameObject( "", assetPath );

                foreach ( Renderer renderer in obj.GetComponentsInChildren< Renderer >() )
                {
                    if ( renderer != null )
                    {
                        foreach ( Material mat in renderer.sharedMaterials )
                        {
                            if ( !textures.Contains( mat.name ) ) textures.Add( mat.name );
                        }
                    }
                }

                UnityEngine.Object.DestroyImmediate( obj );

                progress += 1.0f / max;

                EditorUtility.DisplayProgressBar( $"Texture Sorter", $"Getting Textures ({min++}/{max})", progress );
            }

            progress = 0.0f; min = 0; max = textures.Count;

            foreach ( string texture in textures )
            {
                if ( !File.Exists( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{texture}_albedoTexture.dds" ) )
                {
                    GUIUtility.systemCopyBuffer = texture;

                    if ( !Helper.IsValid( Legion ) ) OpenLegion();

                    string filePath = $"{relativeLegionPlusExportedFiles}/materials/{texture}/{texture}_albedoTexture.dds";
                    string gotoPath = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathMaterials}/{texture}_albedoTexture.dds";

                    EditorUtility.DisplayProgressBar( $"Texture Sorter", $"Processing...", progress );

                    while ( !File.Exists( filePath ) )
                    {
                        await Helper.Wait( 1.0 );

                        EditorUtility.DisplayProgressBar( $"Texture Sorter Paused ({min}/{max})", $"\"{texture}\" need to be imported. ( {max - ( min + 1 )} more missing )", progress );

                        if ( !Helper.IsValid( Legion ) ) break;
                    }

                    // If Legion is closed
                    if ( !File.Exists( filePath ) ) return;

                    File.Move( filePath, gotoPath );
                    if ( File.Exists( $"{filePath}.meta" ) ) File.Move( $"{filePath}.meta", $"{gotoPath}.meta" );

                    string dir = $"{relativeLegionPlusExportedFiles}/materials/{texture}";
                    if ( Directory.Exists( dir ) )
                    {
                        Directory.Delete( dir, true );

                        if ( File.Exists( $"{dir}.meta" ) )
                        {
                            File.Delete( $"{dir}.meta" );
                        }
                    }

                    min++;
                }
            }

            EditorUtility.ClearProgressBar();
        }

        internal static Task DeleteNotUsedTexture()
        {
            List< string > texturesList = new List< string >();

            string[] modeltextureGUID = AssetDatabase.FindAssets( "t:model", new [] { UnityInfo.relativePathModel } );

            int i = 0; int total = modeltextureGUID.Length;
            foreach ( var guid in modeltextureGUID )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guid );
                string[] dependencie = AssetDatabase.GetDependencies( assetPath );
                foreach( string dependencies in dependencie )
                {
                    string fileName = Path.GetFileNameWithoutExtension( dependencies );
                    if ( Path.GetExtension( dependencies ) == ".dds" && !texturesList.Contains( fileName ) )
                    {
                        texturesList.Add( fileName );
                    }
                }

                EditorUtility.DisplayProgressBar( $"Obtaining dependencies {i}/{total}", $"Checking: {guid}", ( i + 1 ) / ( float )total ); i++;
            }

            string[] usedTextures = texturesList.ToArray();

            string[] defaultAssetGUID = AssetDatabase.FindAssets( "t:defaultAsset", new [] { UnityInfo.relativePathMaterials } );
            int j = 0; total = defaultAssetGUID.Length;
            foreach ( var guid in defaultAssetGUID )
            {
                string defaultAssetPath = AssetDatabase.GUIDToAssetPath( guid );

                if ( Path.GetExtension( defaultAssetPath ) == ".dds")
                {
                    File.Delete( defaultAssetPath );
                    File.Delete( defaultAssetPath + ".meta");
                    j++;
                }

                EditorUtility.DisplayProgressBar( $"Checking default assets {j}/{total}", $"Checking: {guid}", ( j + 1 ) / ( float )total ); j++;
            }

            string[] textureGUID = AssetDatabase.FindAssets("t:texture", new [] { UnityInfo.relativePathMaterials });
            int k = 0; total = textureGUID.Length;
            foreach ( var guid in textureGUID )
            {
                string texturePath = AssetDatabase.GUIDToAssetPath( guid );

                if( !usedTextures.Contains(Path.GetFileNameWithoutExtension( texturePath ) ) )
                {
                    File.Delete( texturePath );
                    File.Delete( texturePath + ".meta");
                    k++;
                }

                EditorUtility.DisplayProgressBar( $"Checking textures {k}/{total}", $"Checking: {guid}", ( k + 1 ) / ( float )total ); k++;
            }

            ReMapConsole.Log( $"{j} native assets have been deleted", ReMapConsole.LogType.Success );
            ReMapConsole.Log( $"{k} textures not used have been deleted", ReMapConsole.LogType.Success );
            ReMapConsole.Log( $"Total used textures: {usedTextures.Length} for {modeltextureGUID.Length} models", ReMapConsole.LogType.Info );

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static Task SetScale100ToFBX()
        {
            string[] models = AssetDatabase.FindAssets( "t:Model", new string[] { UnityInfo.relativePathModel } );

            List< ModelImporter > modelImporter = new List< ModelImporter >();

            int i = 0; int total = models.Length;
            foreach ( string model in models )
            {
                string path = AssetDatabase.GUIDToAssetPath( model );
                EditorUtility.DisplayProgressBar( $"Checking FBX Scale {i}/{total}", $"Checking: {Path.GetFileName( path )}", ( i + 1 ) / ( float )models.Length);
                ModelImporter importer = AssetImporter.GetAtPath( path ) as ModelImporter;
                if ( importer != null )
                {
                    importer.globalScale = 100;
                    modelImporter.Add( importer );
                } i++;
            }

            EditorUtility.ClearProgressBar();

            foreach ( ModelImporter model in modelImporter ) model.SaveAndReimport();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return Task.CompletedTask;
        }

        internal static Task FixPrefab( string prefabName )
        {
            string[] prefabs = AssetDatabase.FindAssets( prefabName, new [] { UnityInfo.relativePathPrefabs } );

            int i = 0; int total = prefabs.Length;
            foreach ( string prefab in prefabs )
            {
                string file = AssetDatabase.GUIDToAssetPath( prefab );
                string rpakName = UnityInfo.GetApexModelName( prefabName, true );

                EditorUtility.DisplayProgressBar( $"Fixing Prefabs {i}/{total}", $"Prefab: {prefab}", ( i + 1 ) / ( float )total );

                UnityEngine.GameObject loadedPrefabResource = AssetDatabase.LoadAssetAtPath( file, typeof( UnityEngine.Object ) ) as GameObject;
                if ( loadedPrefabResource == null )
                {
                    ReMapConsole.Log($"[Library Sorter] Error loading prefab: {file}", ReMapConsole.LogType.Error );
                    continue;
                }

                Transform child = loadedPrefabResource.GetComponentsInChildren< Transform >()[1];

                CheckBoxColliderComponent( loadedPrefabResource );

                loadedPrefabResource.transform.position = Vector3.zero;
                loadedPrefabResource.transform.eulerAngles = Vector3.zero;
                child.transform.eulerAngles = FindAnglesOffset( rpakName );
                child.transform.position = Vector3.zero;

                PrefabUtility.SavePrefabAsset( loadedPrefabResource );

                ReMapConsole.Log( $"[Library Sorter] Fixed and saved prefab: {file}", ReMapConsole.LogType.Success ); i++;
            }

            EditorUtility.ClearProgressBar();

            return Task.CompletedTask;
        }

        internal static List< PrefabOffset > FindPrefabOffsetFile()
        {
            string json = System.IO.File.ReadAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathJsonOffset}" );
            return JsonUtility.FromJson< PrefabOffsetList >( json ).List;
        }

        private static Task CreateRpakList()
        {
            RpakContentJson rpakContentJson = CreateRpakContentJson();

            string[] files = Directory.GetFiles( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/all_models", "*.prefab", SearchOption.TopDirectoryOnly ).ToArray();

            int i = 0; int total = files.Length;

            foreach ( string file in files )
            {
                RpakContentClass content = new RpakContentClass();
                content.modelName = Path.GetFileNameWithoutExtension( file );
                content.location = new List< string >();
                string rpakName = "";

                foreach ( RpakData data in libraryData.RpakList )
                {
                    if ( data.Name == RpakManagerWindow.allModelsDataName ) continue;

                    rpakName = UnityInfo.GetApexModelName( content.modelName, true );

                    if ( data.Data.Contains( rpakName ) )
                    {
                        content.location.Add( data.Name );
                    }
                }

                rpakContentJson.List.Add( content );

                EditorUtility.DisplayProgressBar( $"Checking Rpak Content {i}/{total}", $"Checking {content.modelName}", ( i + 1 ) / ( float )total );
                i++;
            }

            EditorUtility.ClearProgressBar();

            string json = JsonUtility.ToJson( rpakContentJson );
            System.IO.File.WriteAllText( $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathPrefabs}/RpakList.json", json );

            return Task.CompletedTask;
        }

        private static RpakContentJson CreateRpakContentJson()
        {
            RpakContentJson rpakContent = new RpakContentJson();
            rpakContent.List = new List< RpakContentClass >();

            return rpakContent;
        }

        private static void SearchPrefabs( string search = "" )
        {
            searchResult = new List< string >();

            foreach ( string prefab in all_model.Data )
            {
                if( search != "" && !prefab.Contains( search ) )
                    continue;
    
                searchResult.Add( prefab );
            }
        }

        internal static void CheckExisting()
        {
            checkExist = CheckDialog( "Check Existing Prefabs", "Do you want check existing prefabs ?" );
        }

        internal static bool DoStartTask()
        {
            return CheckDialog( "Library Sorter", "Are you sure to start this task ?" );
        }

        internal static bool CheckDialog( string title, string content, string trueText = "Yes", string falseText = "No" )
        {
            return EditorUtility.DisplayDialog( title, content, trueText, falseText );
        }
    }
}
