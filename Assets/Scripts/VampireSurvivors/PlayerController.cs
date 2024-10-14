using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using EditorAttributes;

public class PlayerController : MonoBehaviour
{


    [SerializeField] private float moveSpeed;
    private Vector2 moveDir, mousePos;
    private Rigidbody2D rb;

    [FoldoutGroup("Player Actions", nameof(movementAction), nameof(reflectAction), nameof(fireAction), nameof(equipWeapon1Action), nameof(equipWeapon2Action))]
    [SerializeField] private EditorAttributes.Void groupHolder;
    [SerializeField, HideInInspector] private InputActionReference movementAction, reflectAction, fireAction, equipWeapon1Action, equipWeapon2Action;

    public GameObject[] weaponArray;
    [SerializeField] private Weapon currentWeapon;
    private Transform weaponOriginTransform;
    public static event Action playerFireEvent;

    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();
        weaponOriginTransform = GameObject.Find("WeaponOrigin").transform;
    }

    private void Start()
    {
        EquipWeapon(0);
    }

    private void OnEnable()
    {
        fireAction.action.started += Fire;
        reflectAction.action.started += Special;
        equipWeapon1Action.action.started += EquipWeapon1Delegate;
        equipWeapon2Action.action.started += EquipWeapon2Delegate;
    }
    private void OnDisable()
    {
        fireAction.action.started -= Fire;
        reflectAction.action.started -= Special;
        equipWeapon1Action.action.started -= EquipWeapon1Delegate;
        equipWeapon2Action.action.started -= EquipWeapon2Delegate;
    }

    private void EquipWeapon1Delegate(InputAction.CallbackContext context)
    {
        EquipWeapon(0);
    }

    private void EquipWeapon2Delegate(InputAction.CallbackContext context)
    {
        EquipWeapon(1);
    }

    private void Update()
    {
        moveDir = movementAction.action.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);

        Vector2 aimDir = mousePos - rb.position;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90;
        rb.rotation = aimAngle;
    }

    private void Fire(InputAction.CallbackContext obj)
    {

        if (playerFireEvent != null) { playerFireEvent(); }
    }

    private void Special(InputAction.CallbackContext obj)
    {
        print("Special!");
    }

    private void EquipWeapon(int weapon)
    {

        if (currentWeapon != null)
        {
            currentWeapon.UnEquip();
        }

        Weapon weaponToEquip = Instantiate(weaponArray[weapon], weaponOriginTransform.position, weaponOriginTransform.rotation, weaponOriginTransform).GetComponent<Weapon>();

        currentWeapon = weaponToEquip;
        weaponToEquip.Equip();

    }

}
