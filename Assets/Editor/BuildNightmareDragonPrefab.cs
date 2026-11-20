using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildNightmareDragonPrefab
{
    private const string Root = "Assets/Prefabs/Dragon";
    private const string SourcePrefab = "Assets/FourEvilDragonsHP/Prefab/DragonNightmare/Albino.prefab";
    private const string SourceMaterial = "Assets/FourEvilDragonsHP/Materials/DragonNightmare/AlbinoHP.mat";

    [MenuItem("Tools/Dragon Bane/Build Nightmare Combat Prefab")]
    public static void Build()
    {
        EnsureFolder("Assets/Prefabs", "Dragon");
        EnsureFolder(Root, "Materials");
        EnsureFolder(Root, "Animations");
        EnsureFolder(Root, "Controllers");
        EnsureFolder(Root, "Resources");
        EnsureFolder(Root + "/Resources", "Dragons");

        Material material = CreateMaterial();
        AnimationClip idle = CopyClip("idle01", true);
        AnimationClip horn = CopyClip("Horn Attack", false);
        AnimationClip claw = CopyClip("Claw Attack", false);
        AnimationClip die = CopyClip("die", false);

        AddAttackEvents(horn, "StartHornHitbox", "StopHornHitbox");
        AddAttackEvents(claw, "StartClawHitbox", "StopClawHitbox");
        AnimatorController controller = CreateController(idle, horn, claw, die);

        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(SourcePrefab);
        GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        instance.name = "DragonNightmare_Albino_Stage1";

        foreach (SkinnedMeshRenderer renderer in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
                materials[i] = material;
            renderer.sharedMaterials = materials;
        }

        Animator animator = instance.GetComponentInChildren<Animator>(true);
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;

        Transform head = FindDeepChild(instance.transform, "Head") ?? animator.transform;
        Transform rightHand = FindDeepChild(instance.transform, "R_Hand") ?? head;
        GameObject hornHitbox = CreateHitbox(head, "HornHitbox", new Vector3(0f, 0f, 140f), new Vector3(1.8f, 2f, 3.2f));
        GameObject clawHitbox = CreateHitbox(rightHand, "ClawHitbox", Vector3.zero, new Vector3(3.2f, 2.5f, 4.5f));

        NightmareDragonCombatSetup setup = instance.AddComponent<NightmareDragonCombatSetup>();
        setup.Configure(animator, hornHitbox, clawHitbox);

        string prefabPath = Root + "/Resources/Dragons/Stage1.prefab";
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Debug.Log("Nightmare combat prefab created: " + prefabPath);
    }

    private static Material CreateMaterial()
    {
        string path = Root + "/Materials/AlbinoHP_URP.mat";
        Material result = AssetDatabase.LoadAssetAtPath<Material>(path);
        Material source = AssetDatabase.LoadAssetAtPath<Material>(SourceMaterial);
        if (result == null)
        {
            result = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(result, path);
        }
        result.SetTexture("_BaseMap", source.GetTexture("_MainTex"));
        result.SetColor("_BaseColor", source.HasProperty("_Color") ? source.GetColor("_Color") : Color.white);
        EditorUtility.SetDirty(result);
        return result;
    }

    private static AnimationClip CopyClip(string name, bool loop)
    {
        string sourcePath = "Assets/FourEvilDragonsHP/Animations/DragonNightMare/" + name + ".fbx";
        AnimationClip source = null;
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(sourcePath))
            if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__")) source = clip;

        string safeName = name.Replace(" ", "_");
        string path = Root + "/Animations/Nightmare_" + safeName + ".anim";
        AssetDatabase.DeleteAsset(path);
        AnimationClip copy = Object.Instantiate(source);
        copy.name = "Nightmare_" + safeName;
        copy.wrapMode = loop ? WrapMode.Loop : WrapMode.Once;
        AssetDatabase.CreateAsset(copy, path);
        return copy;
    }

    private static void AddAttackEvents(AnimationClip clip, string startFunction, string stopFunction)
    {
        AnimationUtility.SetAnimationEvents(clip, new[]
        {
            new AnimationEvent { time = clip.length * (startFunction == "StartClawHitbox" ? 0.52f : 0.62f), functionName = startFunction },
            new AnimationEvent { time = clip.length * (startFunction == "StartClawHitbox" ? 0.64f : 0.70f), functionName = stopFunction },
            new AnimationEvent { time = clip.length * 0.92f, functionName = "EndAttack" }
        });
        EditorUtility.SetDirty(clip);
    }

    private static AnimatorController CreateController(AnimationClip idle, AnimationClip horn, AnimationClip claw, AnimationClip die)
    {
        string path = Root + "/Controllers/NightmareCombat.controller";
        AssetDatabase.DeleteAsset(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        controller.AddParameter("HornAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("ClawAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Idle"); idleState.motion = idle; machine.defaultState = idleState;
        AnimatorState hornState = machine.AddState("Horn Attack"); hornState.motion = horn;
        AnimatorState clawState = machine.AddState("Claw Attack"); clawState.motion = claw;
        AnimatorState dieState = machine.AddState("Die"); dieState.motion = die;
        AddAnyStateTransition(machine, hornState, "HornAttack");
        AddAnyStateTransition(machine, clawState, "ClawAttack");
        AddAnyStateTransition(machine, dieState, "Die");
        AddExitTransition(hornState, idleState);
        AddExitTransition(clawState, idleState);
        return controller;
    }

    private static void AddAnyStateTransition(AnimatorStateMachine machine, AnimatorState state, string trigger)
    {
        AnimatorStateTransition transition = machine.AddAnyStateTransition(state);
        transition.hasExitTime = false; transition.duration = 0.08f; transition.canTransitionToSelf = false;
        transition.AddCondition(AnimatorConditionMode.If, 0f, trigger);
    }

    private static void AddExitTransition(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition transition = from.AddTransition(to);
        transition.hasExitTime = true; transition.exitTime = 0.95f; transition.duration = 0.08f;
    }

    private static GameObject CreateHitbox(Transform parent, string name, Vector3 position, Vector3 size)
    {
        GameObject hitbox = new GameObject(name);
        hitbox.transform.SetParent(parent, false);
        hitbox.transform.localPosition = position;
        // Nightmare 본 계층이 0.01 배율이므로 월드 히트박스 크기를 보정한다.
        hitbox.transform.localScale = Vector3.one * 100f;
        BoxCollider collider = hitbox.AddComponent<BoxCollider>();
        collider.isTrigger = true; collider.size = size;
        DragonAttackHitbox damage = hitbox.AddComponent<DragonAttackHitbox>();
        damage.Configure(DragonAttackHitbox.AttackType.Charge, 20f);
        hitbox.SetActive(false);
        return hitbox;
    }

    private static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindDeepChild(child, name);
            if (found != null) return found;
        }
        return null;
    }

    private static void EnsureFolder(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, name);
    }
}
