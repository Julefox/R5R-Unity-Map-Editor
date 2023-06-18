
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WindowUtility;
using static WindowUtility.WindowUtility;

namespace CodeViews
{
    public class AdditionalCodeTab : EditorWindow
    {
        private static readonly string    jsonPath    = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathAdditionalCodeJson}";
        private static readonly string    infoPath    = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathAdditionalCodeInfo}";
        internal static readonly string[] contentType = new[] { "HeadContent", "InBlockContent", "BelowContent" };

        internal static readonly string emptyContentStr = "Empty Code";
        private  static string          entry = "";
        internal static bool            isEmptyCode = false;

        internal static bool   showInfo     = false;
        internal static string textInfo     = File.ReadAllText( infoPath );
        internal static string textInfoTemp = textInfo;

        internal static AdditionalCode_ additionalCode;
        internal static AdditionalCodeClass_[] additionalCodeArray;
        internal static AdditionalCodeClass_ activeCode;

        internal static WindowStruct windowStruct = new WindowStruct()
        {
            MainTab = new[] { "Head Code", "In-Block Code", "Below Code" },

            SubTab = new Dictionary< int, string[] >()
            {
                { 0, new[] { "" } },
                { 1, new[] { "" } },
                { 2, new[] { "" } }
            },

            SubTabGUI = new Dictionary< Tuple< int, int >, GUIStruct >()
            {
                // Head Code
                {
                    WindowStruct.NewTuple( 0, 0 ),
                    new GUIStruct()
                    {
                        OnStartGUI = () => SaveJson()
                    }
                },

                // In-Block Code
                {
                    WindowStruct.NewTuple( 1, 0 ),
                    new GUIStruct()
                    {
                        OnStartGUI = () => SaveJson()
                    }
                },

                // Below Code
                {
                    WindowStruct.NewTuple( 2, 0 ),
                    new GUIStruct()
                    {
                        OnStartGUI = () => SaveJson()
                    }
                },
            },

            RefreshCallback = () =>
            {
                GUI.FocusControl( null );
                showInfo = false;
                Refresh( true );
            }
        };

        internal static void MainTab()
        {
            isEmptyCode = activeCode.Content[ windowStruct.SubTabIdx ].Name == emptyContentStr;

            GUILayout.Box( "", GUILayout.ExpandWidth( true ), GUILayout.Height( 2 ) );

            GUILayout.BeginHorizontal();
                if ( !isEmptyCode ) CreateTextField( ref entry, "", "", 1, 200, 20, true );
                if ( !isEmptyCode ) CreateButton( "Rename", "", () => RenameTab(), 100, 20 );

                if ( MenuInit.IsEnable( CodeViewsWindow.DevMenuDebugInfo ) )
                {
                    WindowUtility.WindowUtility.GetEditorWindowSize( CodeViewsWindow.windowInstance );
                    WindowUtility.WindowUtility.GetScrollSize( CodeViewsWindow.scroll );
                }

                FlexibleSpace();

                CreateButton( "Add Code", "", () => AddNewCode(), 100, 20 );
                if ( !isEmptyCode ) CreateButton( "Remove Code", "", () => DeleteCode(), 100, 20 );
                CreateButton( "Save", "", () => SaveJson(), 100, 20 );

                CreateButton( "Info", "", () => { showInfo = !showInfo; }, 100 );
            GUILayout.EndHorizontal();
        }

        internal static void AdditionalCodeInit()
        {
            additionalCode = FindAdditionalCode();

            if ( !Helper.IsValid( additionalCode ) ) return;

            additionalCodeArray = new AdditionalCodeClass_[]
            {
                additionalCode.HeadContent,
                additionalCode.InBlockContent,
                additionalCode.BelowContent
            };

            activeCode = additionalCode.HeadContent;

            Refresh();
        }

        internal static AdditionalCode_ FindAdditionalCode()
        {
            if ( !File.Exists( jsonPath ) )
            {
                CreateNewJsonAdditionalCode();
            }
            else
            {
                string json = System.IO.File.ReadAllText( jsonPath );
                additionalCode = JsonUtility.FromJson< AdditionalCode_ >( json );
            }

            return additionalCode;
        }

        internal static void AddNewCode()
        {
            AdditionalCodeContent_ newContent = NewAdditionalCodeContent();

            activeCode.Content.Add( newContent );

            int position = activeCode.Content.IndexOf( newContent ) - 1;

            if ( position != -1 ) windowStruct.SubTabIdx = position;

            SaveJson();
        }

        internal static void DeleteCode()
        {
            if ( !LibrarySorter.LibrarySorterWindow.CheckDialog( "Delete Code", "Are you sure you want delete this code ?" ) ) return;

            activeCode.Content.Remove( activeCode.Content[ windowStruct.SubTabIdx ] );

            windowStruct.SubTabIdx = windowStruct.SubTab.Count - 1;

            SaveJson();
        }

        internal static void RenameTab()
        {
            if ( string.IsNullOrEmpty( entry ) ) return;

            string newEntry = FindUniqueName( entry );

            activeCode.Content[ windowStruct.SubTabIdx ].Name = newEntry;

            SaveJson();

            for ( int i = 0; i < activeCode.Content.Count - 1; i++ )
            {
                if ( activeCode.Content[ i ].Name == newEntry )
                {
                    windowStruct.SubTabIdx = i;
                    break;
                }
            }

            entry = "";
        }

        internal static void CreateNewJsonAdditionalCode()
        {
            additionalCode = new AdditionalCode_();

            additionalCode.HeadContent = new AdditionalCodeClass_();
            additionalCode.HeadContent.Name = "Head Code";
            additionalCode.HeadContent.Content = new List< AdditionalCodeContent_ >();

            additionalCode.InBlockContent = new AdditionalCodeClass_();
            additionalCode.InBlockContent.Name = "In-Block Code";
            additionalCode.InBlockContent.Content = new List< AdditionalCodeContent_ >();

            additionalCode.BelowContent = new AdditionalCodeClass_();
            additionalCode.BelowContent.Name = "Below Code";
            additionalCode.BelowContent.Content = new List< AdditionalCodeContent_ >();

            AdditionalCodeContent_ emptyContent = NewAdditionalCodeContent();
            emptyContent.Name = emptyContentStr;

            additionalCode.HeadContent.Content.Add( emptyContent );
            additionalCode.InBlockContent.Content.Add( emptyContent );
            additionalCode.BelowContent.Content.Add( emptyContent );

            SaveJson();
        }

        internal static AdditionalCodeContent_ NewAdditionalCodeContent()
        {
            AdditionalCodeContent_ content = new AdditionalCodeContent_();

            content.Name = FindUniqueName( "unnamed" );
            content.Code = "";

            return content;
        }

        private static string FindUniqueName( string name )
        {
            if ( !Helper.IsValid( activeCode ) ) return "";

            string newEntry = name; int idx = 1; bool nameExists;

            do
            {
                nameExists = false;
                foreach ( AdditionalCodeContent_ content in activeCode.Content )
                {
                    if ( content.Name == newEntry )
                    {
                        nameExists = true;
                        newEntry = $"{name} ({++idx})";
                        break;
                    }
                }
            } while ( nameExists );

            return newEntry;
        }

        internal static void Refresh( bool refreshCodeView = false )
        {
            //AdditionalCodeInit();

            if ( !Helper.IsValid( additionalCode ) ) return;

            activeCode = additionalCodeArray[ windowStruct.MainTabIdx ];

            if ( windowStruct.SubTabIdx > activeCode.Content.Count - 1 ) windowStruct.SubTabIdx = activeCode.Content.Count - 1;

            UnityInfo.SortListByKey( activeCode.Content, x => x.Name );

            int targetIndex = -1;

            List< string > pages = new List< string >();

            for ( int i = 0; i < activeCode.Content.Count; i++ )
            {
                AdditionalCodeContent_ content = activeCode.Content[ i ];
                pages.Add( content.Name );

                if ( content.Name == emptyContentStr )
                {
                    targetIndex = i;
                }
            }

            if ( targetIndex != -1 )
            {
                AdditionalCodeContent_ targetContent = activeCode.Content[ targetIndex ];
                activeCode.Content.RemoveAt( targetIndex );
                activeCode.Content.Insert( 0, targetContent );

                pages.RemoveAt( targetIndex );
                pages.Insert( 0, emptyContentStr );
            }

            if ( CodeViewsWindow.windowInstance != null && refreshCodeView ) CodeViewsWindow.Refresh();

            #if ReMapDev
                textInfo = File.ReadAllText( jsonPath );
            #endif

            windowStruct.SubTab[ windowStruct.MainTabIdx ] = pages.ToArray();
        }

        internal static void SaveJson()
        {
            if ( !Helper.IsValid( additionalCode ) ) return;

            string json = JsonUtility.ToJson( additionalCode );
            System.IO.File.WriteAllText( jsonPath, json );

            Refresh();
        }
    }

    [Serializable]
    public class AdditionalCode_
    {
        public AdditionalCodeClass_ HeadContent;
        public AdditionalCodeClass_ InBlockContent;
        public AdditionalCodeClass_ BelowContent;
    }

    [Serializable]
    public class AdditionalCodeClass_
    {
        public string Name;
        public List < AdditionalCodeContent_ > Content;
    }

    [Serializable]
    public class AdditionalCodeContent_
    {
        public string Name;
        public string Code;
    }
}