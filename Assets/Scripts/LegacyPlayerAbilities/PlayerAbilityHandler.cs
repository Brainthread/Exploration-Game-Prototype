using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityHandler : MonoBehaviour
{
    private PlayerAbility m_activePrimary;
    private PlayerAbility m_activeSecondary;
    private PlayerAbility m_activeBanishment;

    private PlayerAbility[] m_secondaryAbilities;


    private GameObject m_primary;
    private GameObject m_secondary;
    private GameObject m_banishment;

    [SerializeField] private GameObject m_camera;
    [SerializeField] private GameObject m_projectileAttack;
    [SerializeField] private GameObject[] m_spells;
    [SerializeField] private GameObject m_banish;

    public Rigidbody m_rigidbody;

    public GameObject Camera { get { return m_camera; } }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Initialize()
    {
        if (m_projectileAttack != null)
        {
            m_primary = Instantiate(m_projectileAttack, transform);
            m_activePrimary = m_projectileAttack.GetComponent<PlayerAbility>();
            m_activePrimary.Initialize(this);
        }
        m_secondaryAbilities = new PlayerAbility[m_spells.Length];
        for (int i = 0; i < m_spells.Length; i++)
        {
            m_spells[i] = Instantiate(m_spells[i], transform);
            m_secondaryAbilities[i] = m_spells[i].GetComponent<PlayerAbility>();
            m_secondaryAbilities[i].Initialize(this);
            m_activeSecondary = m_secondaryAbilities[i];
        }

        if (m_banish != null)
        {
            m_activeBanishment = m_banish.GetComponent<PlayerAbility>();
            m_activeBanishment.Initialize(this);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_primary != null)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                m_activePrimary.TriggerStart();
            }
            if (Input.GetButtonUp("Fire1"))
            {
                m_activePrimary.TriggerStop();
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            m_activeSecondary.TriggerStart();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            m_activeSecondary.TriggerStop();
        }
    }

    void SwitchSecondary()
    {

    }

    public Vector3 GetCurrentSpeed ()
    {
        return m_rigidbody.velocity;
    }

}
