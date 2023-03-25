using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CodeViewsWindow
{
    public class CodeViewsExport
    {
        // FYI
        // TAB \\
        // 0 = Squirrel Code
        // 1 = DataTable Code
        // 2 = Precache Code
        // 3 = Ent Code ( go to ENT TAB )
        // ENT TAB \\
        // 0 = Script Code
        // 1 = Sound Code
        // 2 = Spawn Code

        private static string code;
        private static string functionName;
        private static int tab;
        private static int tab_temp;
        private static int tabEnt;
        private static int tabEnt_temp;
        private static Vector3 StartingOffset;
        private static bool ShowAdvancedMenu;
        private static bool ShowAdvancedMenuTemp;
        private static bool ShowFunction;
        private static bool ShowFunctionTemp;
        private static bool ShowEntFunction;
        private static bool ShowEntFunctionTemp;
        private static int EntFileID;

        private static bool HelperUseStartingOffset;


        [ MenuItem( "ReMap/Export/Script/Function And Map Offset", false, 25 ) ]
        public static async void ExportFunctionAndMapOffset()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = true;
            CodeViewsWindow.ShowFunctionTemp = true;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.ShowEntFunctionTemp = false;
            Helper.UseStartingOffset = true;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Script/Function", false, 25 ) ]
        public static async void ExportFunction()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = true;
            CodeViewsWindow.ShowFunctionTemp = true;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.ShowEntFunctionTemp = false;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Script/Code And Map Offset", false, 25 ) ]
        public static async void ExportCodeAndMapOffset()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowFunctionTemp = false;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.ShowEntFunctionTemp = false;
            Helper.UseStartingOffset = true;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Script/Code", false, 25 ) ]
        public static async void ExportCode()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 0;
            CodeViewsWindow.tab_temp = 0;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowFunctionTemp = false;
            CodeViewsWindow.ShowEntFunction = false;
            CodeViewsWindow.ShowEntFunctionTemp = false;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Ent File/Script", false, 25 ) ]
        public static async void ExportEntFileScript()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 3;
            CodeViewsWindow.tab_temp = 3;
            CodeViewsWindow.tabEnt = 0;
            CodeViewsWindow.tabEnt_temp = 0;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowFunctionTemp = false;
            CodeViewsWindow.ShowEntFunction = true;
            CodeViewsWindow.ShowEntFunctionTemp = true;
            CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Ent File/Sound", false, 25 ) ]
        public static async void ExportEntFileSound()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 3;
            CodeViewsWindow.tab_temp = 3;
            CodeViewsWindow.tabEnt = 1;
            CodeViewsWindow.tabEnt_temp = 1;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowFunctionTemp = false;
            CodeViewsWindow.ShowEntFunction = true;
            CodeViewsWindow.ShowEntFunctionTemp = true;
            CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        [ MenuItem( "ReMap/Export/Ent File/Spawn", false, 25 ) ]
        public static async void ExportEntFileSpawn()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            CodeViewsWindow.tab = 3;
            CodeViewsWindow.tab_temp = 3;
            CodeViewsWindow.tabEnt = 2;
            CodeViewsWindow.tabEnt_temp = 2;
            CodeViewsWindow.StartingOffset = Vector3.zero;
            CodeViewsWindow.ShowAdvancedMenu = false;
            CodeViewsWindow.ShowAdvancedMenuTemp = false;
            CodeViewsWindow.ShowFunction = false;
            CodeViewsWindow.ShowFunctionTemp = false;
            CodeViewsWindow.ShowEntFunction = true;
            CodeViewsWindow.ShowEntFunctionTemp = true;
            CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = false;

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }

        /*
        [ MenuItem( "ReMap/Export/", false, 25 ) ]
        public static async void Export()
        {
            TagHelper.CheckAndCreateTags();

            GetCodeViewsWindowValue();

            //CodeViewsWindow.tab = 
            //CodeViewsWindow.tab_temp = 
            //CodeViewsWindow.tabEnt = 
            //CodeViewsWindow.tabEnt_temp = 
            //CodeViewsWindow.StartingOffset = Vector3.zero;
            //CodeViewsWindow.ShowAdvancedMenu = false;
            //CodeViewsWindow.ShowAdvancedMenuTemp = false;
            //CodeViewsWindow.ShowFunction = 
            //CodeViewsWindow.ShowFunctionTemp = 
            //CodeViewsWindow.ShowEntFunction = 
            //CodeViewsWindow.ShowEntFunctionTemp = 
            //CodeViewsWindow.EntFileID = 27;
            Helper.UseStartingOffset = 

            CodeViewsWindow.Refresh();

            CodeViewsWindow.ExportFunction();

            await Task.Delay( TimeSpan.FromSeconds( 0.01 ) );

            SetCodeViewsWindowValue();
        }
        */

        internal static void GetCodeViewsWindowValue()
        {
            code = CodeViewsWindow.code;
            functionName = CodeViewsWindow.functionName;
            tab = CodeViewsWindow.tab;
            tab_temp = CodeViewsWindow.tab_temp;
            tabEnt = CodeViewsWindow.tabEnt;
            tabEnt_temp = CodeViewsWindow.tabEnt_temp;
            StartingOffset = CodeViewsWindow.StartingOffset;
            ShowAdvancedMenu = CodeViewsWindow.ShowAdvancedMenu;
            ShowAdvancedMenuTemp = CodeViewsWindow.ShowAdvancedMenuTemp;
            ShowFunction = CodeViewsWindow.ShowFunction;
            ShowFunctionTemp = CodeViewsWindow.ShowFunctionTemp;
            ShowEntFunction = CodeViewsWindow.ShowEntFunction;
            ShowEntFunctionTemp = CodeViewsWindow.ShowEntFunctionTemp;
            EntFileID = CodeViewsWindow.EntFileID;
            HelperUseStartingOffset = Helper.UseStartingOffset;
        }

        internal static void SetCodeViewsWindowValue()
        {
            CodeViewsWindow.code = code;
            CodeViewsWindow.functionName = functionName;
            CodeViewsWindow.tab = tab;
            CodeViewsWindow.tab_temp = tab_temp;
            CodeViewsWindow.tabEnt = tabEnt;
            CodeViewsWindow.tabEnt_temp = tabEnt_temp;
            CodeViewsWindow.StartingOffset = StartingOffset;
            CodeViewsWindow.ShowAdvancedMenu = ShowAdvancedMenu;
            CodeViewsWindow.ShowAdvancedMenuTemp = ShowAdvancedMenuTemp;
            CodeViewsWindow.ShowFunction = ShowFunction;
            CodeViewsWindow.ShowFunctionTemp = ShowFunctionTemp;
            CodeViewsWindow.ShowEntFunction = ShowEntFunction;
            CodeViewsWindow.ShowEntFunctionTemp = ShowEntFunctionTemp;
            CodeViewsWindow.EntFileID = EntFileID;
            Helper.UseStartingOffset = HelperUseStartingOffset;

            CodeViewsWindow.Refresh();
        }
    }
}