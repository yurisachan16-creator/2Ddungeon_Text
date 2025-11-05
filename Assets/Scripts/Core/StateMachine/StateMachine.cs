using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机管理类
/// 负责状态的切换和生命周期管理
/// </summary>
public class StateMachine
{
    private IState currentState; // 当前状态
    private Dictionary<System.Type, IState> stateCache = new Dictionary<System.Type, IState>(); // 状态缓存池
    
    /// <summary>
    /// 获取当前状态
    /// </summary>
    public IState CurrentState => currentState;
    
    /// <summary>
    /// 切换到新状态
    /// </summary>
    /// <typeparam name="T">状态类型</typeparam>
    public void ChangeState<T>() where T : IState, new()
    {
        // 如果当前已经是这个状态，则不执行切换
        if (currentState != null && currentState.GetType() == typeof(T))
        {
            return;
        }
        
        // 退出当前状态
        if (currentState != null)
        {
            currentState.OnExit();
        }
        
        // 从缓存获取或创建新状态
        if (!stateCache.TryGetValue(typeof(T), out IState newState))
        {
            newState = new T();
            stateCache.Add(typeof(T), newState);
        }
        
        // 切换到新状态
        currentState = newState;
        currentState.OnEnter();
    }
    
    /// <summary>
    /// 切换到指定状态实例
    /// </summary>
    /// <param name="newState">新状态实例</param>
    public void ChangeState(IState newState)
    {
        if (newState == null)
        {
            Debug.LogWarning("尝试切换到空状态");
            return;
        }
        
        // 如果当前已经是这个状态，则不执行切换
        if (currentState != null && currentState.GetType() == newState.GetType())
        {
            return;
        }
        
        // 退出当前状态
        if (currentState != null)
        {
            currentState.OnExit();
        }
        
        // 切换到新状态
        currentState = newState;
        currentState.OnEnter();
        
        // 将状态加入缓存
        System.Type stateType = newState.GetType();
        if (!stateCache.ContainsKey(stateType))
        {
            stateCache.Add(stateType, newState);
        }
    }
    
    /// <summary>
    /// 更新当前状态（在Update中调用）
    /// </summary>
    public void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }
    
    /// <summary>
    /// 物理更新当前状态（在FixedUpdate中调用）
    /// </summary>
    public void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.OnFixedUpdate();
        }
    }
    
    /// <summary>
    /// 清空状态机
    /// </summary>
    public void Clear()
    {
        if (currentState != null)
        {
            currentState.OnExit();
            currentState = null;
        }
        stateCache.Clear();
    }
}
