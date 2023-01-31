using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForceRecieverRegister : MonoBehaviour
{
    private static ForceRecieverRegister m_current;
    public static ForceRecieverRegister Current { get { return m_current; } }
    [SerializeField] private List<ForceReciever> m_forceRecievers;
    public void Start()
    {
        m_current = this;
    }
    void Update()
    {
        
    }
    public void AddToList (ForceReciever forceReciever)
    {
        m_forceRecievers.Add(forceReciever);
    }
    public void RemoveFromList (ForceReciever forceReciever)
    {
        m_forceRecievers.Remove(forceReciever);
    }
    public void RemoveAtIndex(int position)
    {
        m_forceRecievers.RemoveAt(position);
    }
    public void Clear()
    {
        m_forceRecievers.Clear();
    }
    public List<ForceReciever> Get()
    {
        return m_forceRecievers;
    }
}
