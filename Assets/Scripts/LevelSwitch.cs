using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSwitch : MonoBehaviour
{
    [SerializeField] private int LAYEROFFSET;
    [SerializeField] private List<GameObject> m_staticRealspaceObjects;
    [SerializeField] private List<GameObject> m_staticOtherspaceObjects;

    [SerializeField] private GameObject m_player;
    [SerializeField] private LayerMask m_layerMask;

    [SerializeField] private Material m_realspaceSkybox;
    [SerializeField] private Color m_realspaceShadowColor;
    [SerializeField] private Material m_otherspaceSkybox;
    [SerializeField] private Color m_otherspaceShadowColor;

    private bool m_inRealSpace = true;
    private OtherworldHandler m_otherworldHandler;
    // Start is called before the first frame update
    void Start()
    {
        m_inRealSpace = true;
        m_otherworldHandler = m_player.GetComponent<OtherworldHandler>();
        ChangeVisibility(1, false);
    }

    void ChangeVisibility(int group, bool visible)
    {
        List<GameObject> list = m_staticRealspaceObjects;
        if(group != 0)
        {
            list = m_staticOtherspaceObjects;
        }
        foreach (GameObject obj in list)
        {
            if (obj.GetComponent<Collider>()) obj.GetComponent<Collider>().isTrigger = !visible;
            if (obj.GetComponent<Light>()) obj.GetComponent<Light>().enabled = visible;
            if (visible)
                obj.layer -= LAYEROFFSET;
            else
                obj.layer += LAYEROFFSET;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab)) { VisualizeLevel(); }
        if(Input.GetKeyUp(KeyCode.Tab)) {
            UnVisualizeLevel();
            if(LevelSwitchIsLegal()) SwitchLevel(); 
        }
    }

    private bool LevelSwitchIsLegal()
    {
        return !m_otherworldHandler.IsInsideObjects();
    }

    private void VisualizeLevel()
    {
        List<GameObject> _outList;
        List<GameObject> _inList;

        if (m_inRealSpace)
        {
            _inList = m_staticRealspaceObjects;
            _outList = m_staticOtherspaceObjects;
        }
        else
        {
            _inList = m_staticOtherspaceObjects;
            _outList = m_staticRealspaceObjects;
        }

        foreach (GameObject obj in _outList)
        {
            //obj.layer = 8;
            if(obj.GetComponent<Renderer>())
            {
                MaterialExtensions.ToFadeMode(obj.GetComponent<Renderer>().material);
                Color c = Color.green;
                c.a = 0.2f;
                obj.GetComponent<Renderer>().material.color = c;
            }
        }

        foreach (GameObject obj in _inList)
        {
            if (obj.GetComponent<Renderer>())
            {
                Color c = Color.red;
                c.a = 1f;
                obj.GetComponent<Renderer>().material.color = c;
            }
        }
    }

    private void UnVisualizeLevel()
    {
        List<GameObject> _outList;
        List<GameObject> _inList;

        if (m_inRealSpace)
        {
            _inList = m_staticRealspaceObjects;
            _outList = m_staticOtherspaceObjects;
        }
        else
        {
            _inList = m_staticOtherspaceObjects;
            _outList = m_staticRealspaceObjects;
        }

        foreach (GameObject obj in _outList)
        {
            //obj.layer = 7;
            if (obj.GetComponent<Renderer>())
            {
                MaterialExtensions.ToOpaqueMode(obj.GetComponent<Renderer>().material);
                Color c = Color.white;
                c.a = 1f;
                obj.GetComponent<Renderer>().material.color = c;
            }
        }

        foreach (GameObject obj in _inList)
        {
            if (obj.GetComponent<Renderer>())
            {
                Color c = Color.white;
                c.a = 1f;
                obj.GetComponent<Renderer>().material.color = c;
            }
        }
    }

    public void SwitchLevel()
    {
        m_inRealSpace = !m_inRealSpace;

        if(m_inRealSpace){
            ChangeVisibility(1, false);
            ChangeVisibility(0, true);
            RenderSettings.skybox = m_realspaceSkybox;
            RenderSettings.ambientSkyColor = m_realspaceShadowColor;
        } else
        {
            ChangeVisibility(0, false);
            ChangeVisibility(1, true);
            RenderSettings.skybox = m_otherspaceSkybox;
            RenderSettings.ambientSkyColor = m_otherspaceShadowColor;
        }
    }
}
