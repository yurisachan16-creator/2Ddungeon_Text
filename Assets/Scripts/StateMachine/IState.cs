using UnityEngine;

/// <summary>
/// 状态接口，所有状态类都需要实现此接口
/// </summary>
public interface IState
{
    /// <summary>
    /// 进入状态时调用
    /// </summary>
    void OnEnter();
    
    /// <summary>
    /// 状态运行时每帧调用
    /// </summary>
    void OnUpdate();
    
    /// <summary>
    /// 物理更新时调用（用于移动等物理操作）
    /// </summary>
    void OnFixedUpdate();
    
    /// <summary>
    /// 退出状态时调用
    /// </summary>
    void OnExit();
}
