using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitch : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_staticRealspaceObjects;
    [SerializeField] private List<GameObject> m_staticOtherspaceObjects;

    [SerializeField] private GameObject m_player;
    [SerializeField] private LayerMask m_layerMask;

    [SerializeField] private Material m_realspaceSkybox;
    [SerializeField] private Color m_realspaceShadowColor;
    [SerializeField] private Material m_otherspaceSkybox;
    [SerializeField] private Color m_otherspaceShadowColor;

    private bool m_inRealSpace = false;
    private OtherworldHandler m_otherworldHandler;
    // Start is called before the first frame update
    void Start()
    {
        m_otherworldHandler = m_player.GetComponent<OtherworldHandler>();
        SwitchLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) { VisualizeLevel(); }
        if(Input.GetKeyUp(KeyCode.Tab)&&LevelSwitchIsLegal()) { SwitchLevel(); }
    }

    private bool LevelSwitchIsLegal()
    {
        return !m_otherworldHandler.IsInsideObjects();
    }

    private void VisualizeLevel()
    {
        throw new NotImplementedException();
    }

    public void SwitchLevel()
    {
        m_inRealSpace = !m_inRealSpace;
        List<GameObject> _inList;
        List<GameObject> _outList;

        if(m_inRealSpace){
            _inList = m_staticRealspaceObjects;
            _outList = m_staticOtherspaceObjects;
            RenderSettings.skybox = m_realspaceSkybox;
            RenderSettings.ambientSkyColor = m_realspaceShadowColor;
        } else
        {
            _outList = m_staticRealspaceObjects; 
            _inList= m_staticOtherspaceObjects;
            RenderSettings.skybox = m_otherspaceSkybox;
            RenderSettings.ambientSkyColor = m_otherspaceShadowColor;
        }
        foreach (GameObject obj in _inList) {
            if(obj.GetComponent<Collider>()) obj.GetComponent<Collider>().isTrigger = false;
            if (obj.GetComponent<Light>()) obj.GetComponent<Light>().enabled = true;
            obj.layer = 6;
        }
        foreach (GameObject obj in _outList)
        {
            if (obj.GetComponent<Collider>()) obj.GetComponent<Collider>().isTrigger = true;
            if (obj.GetComponent<Light>()) obj.GetComponent<Light>().enabled = false;
            obj.layer = 7;
        }
    }
}
