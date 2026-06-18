#region Assembly Unity.XR.Interaction.Toolkit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// location unknown
// Decompiled with ICSharpCode.Decompiler 
#endregion

using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class LeapMoveProvider : LocomotionProvider
{
    [SerializeField]
    [Tooltip("The speed, in units per second, to move forward.")]
    private float m_MoveSpeed = 1f;

    [SerializeField]
    [Tooltip("Controls whether to enable strafing (sideways movement).")]
    private bool m_EnableStrafe = true;

    [SerializeField]
    [Tooltip("Controls whether to enable flying (unconstrained movement). This overrides the use of gravity.")]
    private bool m_EnableFly;

    [SerializeField]
    [Tooltip("Controls whether gravity affects this provider when a Character Controller is used and flying is disabled.")]
    private bool m_UseGravity = true;

    [SerializeField]
    [Tooltip("The source Transform to define the forward direction.")]
    private Transform m_ForwardSource;

    [SerializeField]
    [Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
    private XRInputValueReader<float> m_LeftHandMoveInput = new XRInputValueReader<float>("Left Hand Move");

    [SerializeField]
    [Tooltip("Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).")]
    private XRInputValueReader<float> m_RightHandMoveInput = new XRInputValueReader<float>("Right Hand Move");

    private CharacterController m_CharacterController;

    private bool m_AttemptedGetCharacterController;

    private bool m_IsMovingXROrigin;

    private Vector3 m_VerticalVelocity;

    public float moveSpeed
    {
        get
        {
            return m_MoveSpeed;
        }
        set
        {
            m_MoveSpeed = value;
        }
    }

    public bool enableStrafe
    {
        get
        {
            return m_EnableStrafe;
        }
        set
        {
            m_EnableStrafe = value;
        }
    }

    public bool enableFly
    {
        get
        {
            return m_EnableFly;
        }
        set
        {
            m_EnableFly = value;
        }
    }

    public bool useGravity
    {
        get
        {
            return m_UseGravity;
        }
        set
        {
            m_UseGravity = value;
        }
    }

    public Transform forwardSource
    {
        get
        {
            return m_ForwardSource;
        }
        set
        {
            m_ForwardSource = value;
        }
    }

    public XROriginMovement transformation { get; set; } = new XROriginMovement();

    public XRInputValueReader<float> leftHandMoveInput
    {
        get
        {
            return m_LeftHandMoveInput;
        }
        set
        {
            XRInputReaderUtility.SetInputProperty(ref m_LeftHandMoveInput, value, this);
        }
    }

    public XRInputValueReader<float> rightHandMoveInput
    {
        get
        {
            return m_RightHandMoveInput;
        }
        set
        {
            XRInputReaderUtility.SetInputProperty(ref m_RightHandMoveInput, value, this);
        }
    }

    protected void OnEnable()
    {
        m_LeftHandMoveInput.EnableDirectActionIfModeUsed();
        m_RightHandMoveInput.EnableDirectActionIfModeUsed();
    }

    protected void OnDisable()
    {
        m_LeftHandMoveInput.DisableDirectActionIfModeUsed();
        m_RightHandMoveInput.DisableDirectActionIfModeUsed();
    }

    protected void Update()
    {
        m_IsMovingXROrigin = false;
        if (!(base.mediator.xrOrigin?.Origin == null))
        {
            Vector2 vector = ReadInput();
            Vector3 translationInWorldSpace = ComputeDesiredMove(vector.y);
            if (vector != Vector2.zero || m_VerticalVelocity != Vector3.zero)
            {
                MoveRig(translationInWorldSpace);
            }

            if (!m_IsMovingXROrigin)
            {
                TryEndLocomotion();
            }
        }
    }

    private Vector2 ReadInput()
    {
        float vector = m_LeftHandMoveInput.ReadValue();
        float vector2 = m_RightHandMoveInput.ReadValue();
        return new Vector2(0,vector + vector2);
    }

    protected virtual Vector3 ComputeDesiredMove(float inputB)
    {
        Vector2 input = new Vector2(0,inputB);
        if (input == Vector2.zero)
        {
            return Vector3.zero;
        }

        XROrigin xrOrigin = base.mediator.xrOrigin;
        if (xrOrigin == null)
        {
            return Vector3.zero;
        }

        Vector3 vector = Vector3.ClampMagnitude(new Vector3(m_EnableStrafe ? input.x : 0f, 0f, input.y), 1f);
        Transform transform = ((m_ForwardSource == null) ? xrOrigin.Camera.transform : m_ForwardSource);
        Vector3 vector2 = transform.forward;
        Transform transform2 = xrOrigin.Origin.transform;
        float num = m_MoveSpeed;
        if (m_EnableFly)
        {
            Vector3 right = transform.right;
            return (vector.x * right + vector.z * vector2) * num;
        }

        Vector3 up = transform2.up;
        if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(vector2, up)), 1f))
        {
            vector2 = -transform.up;
        }

        Vector3 toDirection = Vector3.ProjectOnPlane(vector2, up);
        Vector3 direction = Quaternion.FromToRotation(transform2.forward, toDirection) * vector * num;
        return transform2.TransformDirection(direction);
    }

    protected virtual void MoveRig(Vector3 translationInWorldSpace)
    {
        if (base.mediator.xrOrigin?.Origin == null)
        {
            return;
        }

        FindCharacterController();
        Vector3 motion = translationInWorldSpace;
        if (m_CharacterController != null && m_CharacterController.enabled)
        {
            if (m_CharacterController.isGrounded || !m_UseGravity || m_EnableFly)
            {
                m_VerticalVelocity = Vector3.zero;
            }
            else
            {
                m_VerticalVelocity += Physics.gravity * Time.deltaTime;
            }

            motion += m_VerticalVelocity;
        }

        TryStartLocomotionImmediately();
        if (base.locomotionState == LocomotionState.Moving)
        {
            m_IsMovingXROrigin = true;
            transformation.motion = motion;
            TryQueueTransformation(transformation);
            /*XROrigin xrOrigin = base.mediator.xrOrigin;
            Transform transform = ((m_ForwardSource == null) ? xrOrigin.Camera.transform : m_ForwardSource);
            XRBodyGroundPosition positionTransformation = new XRBodyGroundPosition();
            positionTransformation.targetPosition = xrOrigin.Camera.transform.position + translationInWorldSpace;
            TryQueueTransformation(positionTransformation);*/
        }
    }

    private void FindCharacterController()
    {
        GameObject gameObject = base.mediator.xrOrigin?.Origin;
        if (!(gameObject == null) && m_CharacterController == null && !m_AttemptedGetCharacterController)
        {
            if (!gameObject.TryGetComponent<CharacterController>(out m_CharacterController) && gameObject != base.mediator.xrOrigin.gameObject)
            {
                base.mediator.xrOrigin.TryGetComponent<CharacterController>(out m_CharacterController);
            }

            m_AttemptedGetCharacterController = true;
        }
    }
}
#if false // Decompilation log
'266' items in cache
------------------
Resolve: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Found single assembly: 'netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/NetStandard/ref/2.1.0/netstandard.dll'
------------------
Resolve: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll'
------------------
Resolve: 'UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.dll'
------------------
Resolve: 'Unity.InputSystem, Version=1.19.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.InputSystem, Version=1.19.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/run/media/system/MORE_SPACE/git/MedoDeCrime/Library/ScriptAssemblies/Unity.InputSystem.dll'
------------------
Resolve: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.XRModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.XRModule.dll'
------------------
Resolve: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AnimationModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.AnimationModule.dll'
------------------
Resolve: 'UnityEngine.SpatialTracking, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'UnityEngine.SpatialTracking, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/run/media/system/MORE_SPACE/git/MedoDeCrime/Library/ScriptAssemblies/UnityEngine.UI.dll'
------------------
Resolve: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.UIModule.dll'
------------------
Resolve: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.AudioModule.dll'
------------------
Resolve: 'Unity.XR.CoreUtils, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.XR.CoreUtils, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/run/media/system/MORE_SPACE/git/MedoDeCrime/Library/ScriptAssemblies/Unity.XR.CoreUtils.dll'
------------------
Resolve: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.PhysicsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.PhysicsModule.dll'
------------------
Resolve: 'Unity.Mathematics, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'Unity.Mathematics, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/run/media/system/MORE_SPACE/git/MedoDeCrime/Library/ScriptAssemblies/Unity.Mathematics.dll'
------------------
Resolve: 'Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'Unity.Burst, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.InputLegacyModule.dll'
------------------
Resolve: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEngine.Physics2DModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEngine.Physics2DModule.dll'
------------------
Resolve: 'Unity.XR.OpenXR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Could not find by name: 'Unity.XR.OpenXR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Resolve: 'UnityEditor.QuickSearchModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'UnityEditor.QuickSearchModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/Managed/UnityEngine/UnityEditor.QuickSearchModule.dll'
------------------
Resolve: 'System.Runtime.InteropServices, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'System.Runtime.InteropServices, Version=4.1.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '2.1.0.0', Got: '4.1.2.0'
Load from: '/var/home/Nuk/Unity/Hub/Editor/6000.0.72f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.InteropServices.dll'
------------------
Resolve: 'System.Runtime.CompilerServices.Unsafe, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null'
Found single assembly: 'System.Runtime.CompilerServices.Unsafe, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
WARN: Version mismatch. Expected: '2.1.0.0', Got: '6.0.0.0'
Load from: '/run/media/system/MORE_SPACE/git/MedoDeCrime/Library/PackageCache/com.unity.collections@12999e356c23/Unity.Collections.Tests/System.Runtime.CompilerServices.Unsafe/System.Runtime.CompilerServices.Unsafe.dll'
#endif
