using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class AnimationPlayerEditor : EditorWindow {
    //List<List<AnimationClip>> listClipByAnimator = new List<List<AnimationClip>>(); 
    public Scene scene;

    public List<Animator> animatorList = new List<Animator>();
    public List<AnimationClip>[] arrayAnimClipList;
    public List<int> dropDownIndexList = new List<int>();
    public List<float> speedSliderList = new List<float>();
    public List<float> sampleSliderList = new List<float>();
    public List<float> currentDurationList = new List<float>();
    public List<float> totalDurationList = new List<float>();
    public List<bool> isLoopList = new List<bool>();
    public List<float> timerList = new List<float>();
    public string searchString;
    public string[][] animationDropDown;
    public Vector2 scrollPosition = Vector2.zero;
    public List<float> times = new List<float>();

    int state = 0;
    int index;
    int animIndex;
    static AnimationPlayerEditor window;
    [MenuItem("TP/AnimatorWindow")]

    static void InitWindow() {
        Debug.Log("init window");
        window = GetWindow<AnimationPlayerEditor>();
        window.Show();
        window.titleContent = new GUIContent("AnimatorWindow");
        window.scene = SceneManager.GetActiveScene();
        window.animatorList = window.GetAllAnimator();
        window.arrayAnimClipList = new List<AnimationClip>[window.animatorList.Count];
        window.animationDropDown = new string[window.animatorList.Count][];
        window.maxSize = new Vector2(400, 600);
        EditorApplication.update += EditorUpdate;
        EditorApplication.playModeStateChanged += StateChange;

        for (int i = 0; i < window.animatorList.Count; i++) {
            window.dropDownIndexList.Add(0);
            window.speedSliderList.Add(1f);
            window.sampleSliderList.Add(0f);
            window.currentDurationList.Add(0f);
            window.totalDurationList.Add(0f);
            window.isLoopList.Add(false);
            window.timerList.Add(1f);
            window.times.Add(0f);

            window.arrayAnimClipList[i] = new List<AnimationClip>();
            window.arrayAnimClipList[i] = window.GetAllClipFromAnimator(window.animatorList[i]);
            window.animationDropDown[i] = new string[window.arrayAnimClipList[i].Count];
            for (int j = 0; j < window.arrayAnimClipList[i].Count; j++) {
                window.animationDropDown[i][j] = window.arrayAnimClipList[i][j].name;
            }
        }
    }

    private void OnGUI() {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Width(400), GUILayout.Height(600));

        searchString = GUILayout.TextField(searchString);


        switch (state) {
            case 0:
                GUILayout.Label("Select window type");
                if (GUILayout.Button("Window A")) {
                    state = 1;
                }
                if (GUILayout.Button("Window B")) {
                    state = 2;
                }
                break;
            case 1:
                for (int i = 0; i < animatorList.Count; i++) {

                    if (GUILayout.Button("Focus on " + animatorList[i].gameObject.name)) {
                        Selection.activeGameObject = animatorList[i].gameObject;
                        SceneView.FrameLastActiveSceneView();
                    }

                    GUILayout.Label("Select animation to play");

                    if (animationDropDown != null)
                        dropDownIndexList[i] = EditorGUILayout.Popup(dropDownIndexList[i], animationDropDown[i]);
                    else
                        dropDownIndexList[i] = EditorGUILayout.Popup(dropDownIndexList[i], new string[0]); ;


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Animation Speed");
                    GUILayout.FlexibleSpace();
                    speedSliderList[i] = EditorGUILayout.Slider(speedSliderList[i], 0.0f, 3f);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Sample animation");
                    GUILayout.FlexibleSpace();

                    if (arrayAnimClipList != null)
                        sampleSliderList[i] = EditorGUILayout.Slider(sampleSliderList[i], 0.0f, arrayAnimClipList[i][window.dropDownIndexList[i]].length);
                    else
                        sampleSliderList[i] = EditorGUILayout.Slider(sampleSliderList[i], 0.0f, 1);

                    GUILayout.EndHorizontal();

                    if (GUILayout.Button("Play")) {
                        if (animationDropDown != null)
                            PlayAnimation(animatorList[i], animationDropDown[i], dropDownIndexList[i]);
                    }
                    if (GUILayout.Button("Stop")) {
                        if (animationDropDown != null)
                            StopAnimation(animatorList[i], animationDropDown[i], dropDownIndexList[i]);
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Total animation duration");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(totalDurationList[i].ToString());
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Current animation duration");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(currentDurationList[i].ToString());
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Is animation looped ?");
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(isLoopList[i].ToString());
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Delay between replay");
                    GUILayout.FlexibleSpace();
                    timerList[i] = EditorGUILayout.FloatField(timerList[i]);
                    GUILayout.EndHorizontal();

                    GUILayout.Label(" ");
                    if (GUILayout.Button("Retour")) {
                        state = 0;
                    }
                }

                break;
            case 2:
                for (int i = 0; i < animatorList.Count; i++) {
                    if (GUILayout.Button(animatorList[i].gameObject.name)) {
                        state = 3;
                        index = i;
                    }
                }
                if (GUILayout.Button("Retour")) {
                    state = 0;
                }
                break;
            case 3:
                for (int i = 0; i < arrayAnimClipList[index].Count; i++) {
                    if (GUILayout.Button(arrayAnimClipList[index][i].name)) {
                        state = 999;
                        animIndex = i;
                    }
                }
                if (GUILayout.Button("Retour")) {
                    state = 2;
                }
                break;
            case 999:
                GUILayout.Label(arrayAnimClipList[index][animIndex].name);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Animation Speed");
                GUILayout.FlexibleSpace();
                speedSliderList[index] = EditorGUILayout.Slider(speedSliderList[index], 0.0f, 3f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Sample animation");
                GUILayout.FlexibleSpace();

                if (arrayAnimClipList != null)
                    sampleSliderList[index] = EditorGUILayout.Slider(sampleSliderList[index], 0.0f, arrayAnimClipList[index][window.dropDownIndexList[index]].length);
                else
                    sampleSliderList[index] = EditorGUILayout.Slider(sampleSliderList[index], 0.0f, 1);

                GUILayout.EndHorizontal();

                if (GUILayout.Button("Play")) {
                    animatorList[index].Play(arrayAnimClipList[index][animIndex].name, -1, 0f);
                    if (!EditorApplication.isPlaying)
                        animatorList[index].enabled = true;
                }
                if (GUILayout.Button("Stop")) {
                    if (!EditorApplication.isPlaying)
                        animatorList[index].enabled = false;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Total animation duration");
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(totalDurationList[index].ToString());
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Current animation duration");
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(currentDurationList[index].ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Is animation looped ?");
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(isLoopList[index].ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Delay between replay");
                GUILayout.FlexibleSpace();
                timerList[index] = EditorGUILayout.FloatField(timerList[index]);
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Focus")) {
                    Selection.activeGameObject = animatorList[index].gameObject;
                    SceneView.FrameLastActiveSceneView();
                }
                if (GUILayout.Button("Retour")) {
                    state = 3;
                }
                break;
            default:
                break;
        }


        GUILayout.EndScrollView();
    }

    private List<AnimationClip> GetAllClipFromAnimator(Animator animator) {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        List<AnimationClip> listClips = new List<AnimationClip>();
        foreach (AnimationClip clip in clips) {
            listClips.Add(clip);
        }

        return listClips;
    }

    private List<Animator> GetAllAnimator() {
        List<Animator> animatorsInScene = new List<Animator>();
        GameObject[] sceneGameObject = scene.GetRootGameObjects();
        foreach (GameObject gameObject in sceneGameObject) {
            Animator a = gameObject.GetComponent<Animator>();
            if (a != null) {
                animatorsInScene.Add(a);
                a.enabled = false;
            }
        }

        return animatorsInScene;
    }

    private void PlayAnimation(Animator animator, string[] animArray, int index) {
        //string animName = animArray[index];
        //animator.Play(animName, -1, 0f);
        if (!EditorApplication.isPlaying)
            animator.enabled = true;
    }

    private void StopAnimation(Animator animator, string[] animArray, int index) {
        if (!EditorApplication.isPlaying)
            animator.enabled = false;
    }

    private static void EditorUpdate() {
        if (!EditorApplication.isPlaying) {
            Debug.Log(window.searchString);
            for (int i = 0; i < window.animatorList.Count; i++) {
                window.times[i] += Time.deltaTime;
                if (window.times[i] >= window.timerList[i]) {
                    window.animatorList[i].Update(Time.deltaTime);

                }
                window.animatorList[i].speed = window.speedSliderList[i];

                if (!window.animatorList[i].enabled)
                    window.arrayAnimClipList[i][window.dropDownIndexList[i]].SampleAnimation(window.animatorList[i].gameObject, window.sampleSliderList[i]);

                window.totalDurationList[i] = window.arrayAnimClipList[i][window.dropDownIndexList[i]].length;
                window.currentDurationList[i] = window.arrayAnimClipList[i][window.dropDownIndexList[i]].length * window.animatorList[i].GetCurrentAnimatorStateInfo(0).normalizedTime;

                window.isLoopList[i] = window.arrayAnimClipList[i][window.dropDownIndexList[i]].isLooping;

            }
        }
    }


    private static void StateChange(PlayModeStateChange state) {
        switch (state) {
            case PlayModeStateChange.EnteredEditMode:

                break;
            case PlayModeStateChange.ExitingEditMode:
                foreach (Animator animator in window.animatorList) {
                    animator.enabled = true;
                    animator.Play("Entry", -1, 0);
                }
                break;
            case PlayModeStateChange.EnteredPlayMode:

                break;
            case PlayModeStateChange.ExitingPlayMode:
                foreach (Animator animator in window.animatorList) {
                    animator.enabled = false;
                }
                break;
            default:
                break;
        }
    }
}
