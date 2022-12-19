using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class CodeImport : EditorWindow
{
    static string text = "";
    static Vector2 scroll;

    [MenuItem("R5Reloaded/Import Map Code", false, 100)]
    static void Init()
    {
        CodeImport window = (CodeImport)GetWindow(typeof(CodeImport), false, "Import Map Code");
        window.Show();
    }

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        text = GUILayout.TextArea(text, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Import Map Code"))
            ImportMapCode();
    }

    void ImportMapCode()
    {
        List<String> Props = new List<String>();
        List<String> JumpPads = new List<String>();
        List<String> BubbleSheilds = new List<String>();
        List<String> WeaponRacks = new List<String>();
        List<String> LootBins = new List<String>();
        List<String> Ziplines = new List<String>();
        List<String> LinkedZiplines = new List<String>();
        List<String> Doors = new List<String>();
        List<String> Triggers = new List<String>();

        string[] lines = text.Split(char.Parse("\n"));
        foreach(string line in lines) {
            if(line.Contains("JumpPad_CreatedCallback("))
                JumpPads.Add(line.Replace("JumpPad_CreatedCallback( MapEditor_CreateProp(", "").Replace(" ) )", "").Replace(" ", ""));
            else if (line.Contains("MapEditor_CreateProp("))
                Props.Add(line.Replace("MapEditor_CreateProp(", "").Replace(" )", "").Replace(" ", ""));
            else if (line.Contains("MapEditor_CreateBubbleShieldWithSettings("))
                BubbleSheilds.Add(line.Replace("MapEditor_CreateBubbleShieldWithSettings(", "").Replace(" )", ""));
            else if (line.Contains("MapEditor_CreateRespawnableWeaponRack("))
                WeaponRacks.Add(line.Replace("MapEditor_CreateRespawnableWeaponRack(", "").Replace(" )", "").Replace(" ", ""));
            else if (line.Contains("MapEditor_CreateLootBin("))
                LootBins.Add(line.Replace("MapEditor_CreateLootBin(", "").Replace(" )", "").Replace(" ", ""));
            else if (line.Contains("CreateZipline("))
                Ziplines.Add(line.Replace("CreateZipline(", "").Replace(" )", "").Replace(" ", ""));
            else if (line.Contains("MapEditor_CreateLinkedZipline("))
                LinkedZiplines.Add(line.Replace("MapEditor_CreateLinkedZipline(", "").Replace(" )", "").Replace(" ", ""));
            else if (line.Contains("MapEditor_SpawnDoor("))
                Doors.Add(line.Replace("MapEditor_SpawnDoor(", "").Replace(" )", "").Replace(" ", ""));
            else if (line.Contains("MapEditor_CreateTrigger("))
                Triggers.Add(line.Replace("MapEditor_CreateTrigger(", "").Replace(" )", "").Replace(" ", ""));
        }

        if(Props.Count > 0) {
            GameObject objToSpawn = new GameObject("Props");
            objToSpawn.name = "Props";
        }

        if(JumpPads.Count > 0) {
            GameObject objToSpawn = new GameObject("JumpPads");
            objToSpawn.name = "JumpPads";
        }

        if(BubbleSheilds.Count > 0) {
            GameObject objToSpawn = new GameObject("BubbleSheilds");
            objToSpawn.name = "BubbleSheilds";
        }

        if(WeaponRacks.Count > 0) {
            GameObject objToSpawn = new GameObject("WeaponRacks");
            objToSpawn.name = "WeaponRacks";
        }

        if(LootBins.Count > 0) {
            GameObject objToSpawn = new GameObject("LootBins");
            objToSpawn.name = "LootBins";
        }

        if(Ziplines.Count > 0 || LinkedZiplines.Count > 0) {
            GameObject objToSpawn = new GameObject("Ziplines");
            objToSpawn.name = "Ziplines";
        }

        if(Doors.Count > 0) {
            GameObject objToSpawn = new GameObject("Doors");
            objToSpawn.name = "Doors";
        }

        if(Triggers.Count > 0) {
            GameObject objToSpawn = new GameObject("Triggers");
            objToSpawn.name = "Triggers";
        }

        ImportProps(Props);
        ImportJumpPads(JumpPads);
        ImportBubbleSheilds(BubbleSheilds);
        ImportWeaponRacks(WeaponRacks);
        ImportLootBins(LootBins);
        ImportZiplines(Ziplines, LinkedZiplines);
        ImportDoors(Doors);
        ImportTriggers(Triggers);
    }

    void ImportProps(List<String> Props)
    {
        if(Props.Count == 0)
            return;
        
        foreach(string prop in Props)
        {
            string[] split = prop.Split(char.Parse(","));
            if (split.Length < 11)
                continue;

            string Model = split[0].Replace("/", "#").Replace(".rmdl", "").Replace("\"", "").Replace("\n", "").Replace("\r", "").Replace("$", "");

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[2]), float.Parse(split[3].Replace(">", "")), -(float.Parse(split[1].Replace("<", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[4].Replace("<", ""))), -(float.Parse(split[5])), float.Parse(split[6].Replace(">", "")));
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(float.Parse(split[10]), float.Parse(split[10]), float.Parse(split[10]));

            PropScript script = obj.GetComponent<PropScript>();
            script.fadeDistance = int.Parse(split[8]);
            script.allowMantle = bool.Parse(split[7]);
            script.realmID = int.Parse(split[9]);

            GameObject parent = GameObject.Find("Props");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }

    void ImportBubbleSheilds(List<String> BubbleSheilds)
    {
        if(BubbleSheilds.Count == 0)
            return;
        
        foreach(string bsheild in BubbleSheilds)
        {
            string[] split = bsheild.Split(char.Parse(","));
            if (split.Length < 9)
                continue;

            string Model = split[8].Replace("/", "#").Replace(".rmdl", "").Replace("\"", "").Replace("\n", "").Replace("\r", "").Replace("$", "").Replace(" ", "");

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[1].Replace(" ", "")), float.Parse(split[2].Replace(">", "").Replace(" ", "")), -(float.Parse(split[0].Replace("<", "").Replace(" ", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[3].Replace("<", "").Replace(" ", ""))), -(float.Parse(split[4].Replace(" ", ""))), float.Parse(split[5].Replace(">", "").Replace(" ", "")));
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(float.Parse(split[6].Replace(" ", "")), float.Parse(split[6].Replace(" ", "")), float.Parse(split[6].Replace(" ", "")));

            string[] colorsplit = split[7].Split(char.Parse(" "));
            BubbleScript script = obj.GetComponent<BubbleScript>();
            script.shieldColor.r = byte.Parse(colorsplit[1].Replace("\"", ""));
            script.shieldColor.g = byte.Parse(colorsplit[2].Replace("\"", ""));
            script.shieldColor.b = byte.Parse(colorsplit[3].Replace("\"", ""));

            GameObject parent = GameObject.Find("BubbleSheilds");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }

    void ImportJumpPads(List<String> JumpPads)
    {
        if(JumpPads.Count == 0)
            return;
        
        foreach(string jumppad in JumpPads)
        {
            string[] split = jumppad.Split(char.Parse(","));
            if (split.Length < 7)
                continue;

            string Model = "custom_jumppad";

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[2]), float.Parse(split[3].Replace(">", "")), -(float.Parse(split[1].Replace("<", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[4].Replace("<", ""))), -(float.Parse(split[5])), float.Parse(split[6].Replace(">", "")));
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(float.Parse(split[10]), float.Parse(split[10]), float.Parse(split[10]));

            PropScript script = obj.GetComponent<PropScript>();
            script.fadeDistance = int.Parse(split[8]);
            script.allowMantle = bool.Parse(split[7]);
            script.realmID = int.Parse(split[9]);

            GameObject parent = GameObject.Find("JumpPads");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }

    void ImportLootBins(List<String> LootBins)
    {
        if(LootBins.Count == 0)
            return;
        
        foreach(string bin in LootBins)
        {
            string[] split = bin.Split(char.Parse(","));
            if (split.Length < 7)
                continue;

            string Model = "custom_lootbin";

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2].Replace(">", "")), -(float.Parse(split[0].Replace("<", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[3].Replace("<", ""))), -(float.Parse(split[4])), float.Parse(split[5].Replace(">", "")));
            obj.name = Model;

            LootBinScript script = obj.GetComponent<LootBinScript>();
            script.lootbinSkin = int.Parse(split[6]);

            GameObject parent = GameObject.Find("LootBins");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }

    void ImportWeaponRacks(List<String> WeaponRacks)
    {
        if(WeaponRacks.Count == 0)
            return;
        
        foreach(string wrack in WeaponRacks)
        {
            string[] split = wrack.Split(char.Parse(","));
            if (split.Length < 8)
                continue;

            string Model = split[6].Replace("\"", "").Replace("mp_weapon_", "custom_weaponrack_");

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2].Replace(">", "")), -(float.Parse(split[0].Replace("<", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[3].Replace("<", ""))), -(float.Parse(split[4])), float.Parse(split[5].Replace(">", "")));
            obj.name = Model;

            WeaponRackScript script = obj.GetComponent<WeaponRackScript>();
            script.respawnTime = int.Parse(split[7]);

            GameObject parent = GameObject.Find("WeaponRacks");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }

    void ImportZiplines(List<String> ZipLines, List<String> LinkedZiplines)
    {
        if (ZipLines.Count != 0)
        {
            foreach (string zip in ZipLines)
            {
                string[] split = zip.Split(char.Parse(","));
                if (split.Length < 6)
                    continue;

                string Model = "custom_zipline";

                //Find Model GUID in Assets
                string[] results = AssetDatabase.FindAssets(Model);
                if (results.Length == 0)
                    continue;

                //Get model path from guid and load it
                UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
                if (loadedPrefabResource == null)
                    continue;

                GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
                foreach (Transform child in obj.transform)
                {
                    if (child.name == "zipline_start")
                        child.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2].Replace(">", "")), -(float.Parse(split[0].Replace("<", ""))));
                    else if (child.name == "zipline_end")
                        child.transform.position = new Vector3(float.Parse(split[4]), float.Parse(split[5].Replace(">", "")), -(float.Parse(split[3].Replace("<", ""))));
                }

                GameObject parent = GameObject.Find("Ziplines");
                if (parent != null)
                    obj.gameObject.transform.parent = parent.transform;
            }
        }

        if (LinkedZiplines.Count != 0)
        {
            foreach(string zip in LinkedZiplines)
            {
                bool isSmoothed = false;
                bool smoothingType = false;
                int smoothAmount = 0;

                if(zip.Contains("GetAllPointsOnBezier"))
                {
                    isSmoothed = true;
                    smoothingType = true;
                }
                else if(zip.Contains("GetBezierOfPath"))
                {
                    isSmoothed = true;
                    smoothingType = false;
                }

                string finishedzip = zip.Replace("GetAllPointsOnBezier(", "").Replace("GetBezierOfPath(", "").Replace(")", "").Replace("[", "").Replace("]", "").Replace(">,<", ":");
                string[] split = finishedzip.Split(char.Parse(":"));

                GameObject obj = new GameObject("custom_linked_zipline");
                obj.AddComponent<DrawLinkedZipline>();
                obj.AddComponent<LinkedZiplineScript>();

                int i = 0;
                foreach(string s in split)
                {
                    string[] split2 = s.Split(char.Parse(","));

                    if (i == split.Length - 1 && isSmoothed)
                        smoothAmount = int.Parse(split2[3]);

                    GameObject child = new GameObject("zipline_node");
                    child.transform.position = new Vector3(float.Parse(split2[1]), float.Parse(split2[2].Replace(">", "")), -(float.Parse(split2[0].Replace("<", ""))));
                    child.transform.parent = obj.transform;
                    
                    i++;
                }

                LinkedZiplineScript script = obj.GetComponent<LinkedZiplineScript>();
                script.enableSmoothing = isSmoothed;
                script.smoothType = smoothingType;
                script.smoothAmount = smoothAmount;

                GameObject parent = GameObject.Find("Ziplines");
                if (parent != null)
                    obj.gameObject.transform.parent = parent.transform;
            }   
        }
    }

    void ImportDoors(List<String> Doors)
    {
        if(Doors.Count == 0)
            return;
        
        foreach(string door in Doors)
        {
            string[] split = door.Split(char.Parse(","));

            string Model = "custom_single_door";
            bool IsSingleOrDouble = false;
            switch(split[6])
            {
                case "eMapEditorDoorType.Single":
                    Model = "custom_single_door";
                    IsSingleOrDouble = true;
                    break;
                case "eMapEditorDoorType.Double":
                    Model = "custom_double_door";
                    IsSingleOrDouble = true;
                    break;
                case "eMapEditorDoorType.Vertical":
                    Model = "custom_vertical_door";
                    break;
                case "eMapEditorDoorType.Horizontal":
                    Model = "custom_sliding_door";
                    break;
            }

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2].Replace(">", "")), -(float.Parse(split[0].Replace("<", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[3].Replace("<", ""))), -(float.Parse(split[4])), float.Parse(split[5].Replace(">", "")));
            obj.name = Model;

            if(IsSingleOrDouble)
            {
                DoorScript script = obj.GetComponent<DoorScript>();
                script.goldDoor = bool.Parse(split[7]);
            }

            GameObject parent = GameObject.Find("Doors");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }

    void ImportTriggers(List<String> Triggers)
    {
        if(Triggers.Count == 0)
            return;
        
        foreach(string trigger in Triggers)
        {
            string[] split1 = trigger.Split(char.Parse("="));
            string[] split = split1[1].Split(char.Parse(","));

            string Model = "trigger_cylinder";

            //Find Model GUID in Assets
            string[] results = AssetDatabase.FindAssets(Model);
            if (results.Length == 0)
                continue;

            //Get model path from guid and load it
            UnityEngine.Object loadedPrefabResource = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(UnityEngine.Object)) as GameObject;
            if (loadedPrefabResource == null)
                continue;

            GameObject obj = PrefabUtility.InstantiatePrefab(loadedPrefabResource as GameObject) as GameObject;
            obj.transform.position = new Vector3(float.Parse(split[1]), float.Parse(split[2].Replace(">", "")), -(float.Parse(split[0].Replace("<", ""))));
            obj.transform.eulerAngles = new Vector3(-(float.Parse(split[3].Replace("<", ""))), -(float.Parse(split[4])), float.Parse(split[5].Replace(">", "")));
            obj.name = Model;
            obj.gameObject.transform.localScale = new Vector3(float.Parse(split[6]), float.Parse(split[7]), float.Parse(split[6]));

            TriggerScripting script = obj.GetComponent<TriggerScripting>();
            script.Debug = bool.Parse(split[8]);

            GameObject parent = GameObject.Find("Triggers");
            if (parent != null)
                obj.gameObject.transform.parent = parent.transform;
        }
    }
}