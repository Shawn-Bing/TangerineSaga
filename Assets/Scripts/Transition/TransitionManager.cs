using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//自带场景管理，提供方法

namespace T_Saga.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]//使用Scene Name Attribute
        public string startSceneName = string.Empty;//初始场景
        public Vector3 startPosition;//初始位置
        

        #region 注册切换场景事件
        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName,startPosition));
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            StartCoroutine(TransitScene(sceneToGo, positionToGo));
        }
        #endregion


        /// <summary>
        /// （协程）加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="targetPosition">设定人物位置</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName,Vector3 targetPosition)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            // 获取引擎中所有场景
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);

            // 移动人物坐标
            EventHandler.CallMoveToPosition(targetPosition);
            EventHandler.CallAfterSceneLoadEvent();
        }


        /// <summary>
        /// 切换场景
        /// 先Unload当前场景再SetActive另一个场景
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>
        private IEnumerator TransitScene(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();//通知程序在卸载场景之前，要执行如下方法
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            yield return LoadSceneSetActive(sceneName,targetPosition);
        }
    }
}