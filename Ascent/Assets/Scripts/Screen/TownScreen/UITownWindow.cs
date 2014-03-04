//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;

public class UITownWindow : UIPlayerMenuWindow
{
	Transform sharedEle;
	public Transform pointerTransform;
	protected float pointerAngle = 0f;
	public float PointerAngle
	{
		get 
		{
			// refresh value whenever it is requested
			pointerAngle = Utilities.VectorToAngleInDegrees(player.Input.LeftStickX.Value, player.Input.LeftStickY.Value);
			return pointerAngle;
			//return Utilities.VectorToAngleInDegrees(pointerTransform.forward.x - pointerTransform.position.x, pointerTransform.forward.y - pointerTransform.position.y);
		}
	}

	[HideInInspector]
	public UILabel TitleLabel = null;
	[HideInInspector]
	public UILabel InfoLabel = null;
	[HideInInspector]
	public UILabel InstructLabel = null;

	Spin spinScript = null;
	GameObject cardBack = null;
	GameObject confirmBox = null;
	GameObject noticeBox = null;
	public delegate void PopupEvent(bool result);
	public event PopupEvent PopupClose ;

	int transitionTarget = -1;
	/// <summary>
	/// 0 = idle,
	/// 1 = start forward,
	/// 2 = forward,
	/// 3 = start reverse,
	/// 4 = reverse,
	/// </summary>
	int flipState = 0; 
	int confirmState = 0;
	int noticeState = 0;
	
	protected UITownScreen townScreen;


	public bool PopupActive
	{
		get
		{
			return (flipState > 0 || confirmState > 0 || noticeState > 0); 
		}

	}

	public enum EBackpackPanels
	{
		TOWN,
		BACKPACK,
		TOWER,
		SKILLS,
		ACCSHOP,
		CONSHOP,
		TAVERN,
		CHAPEL,
		MAX
	}

	public override void Initialise ()
	{
		spinScript = GetComponent<Spin>();
		sharedEle = transform.Find("Shared Elements");
		
		cardBack = sharedEle.Find("CardBack").gameObject;
		confirmBox = sharedEle.Find("Confirmation").gameObject;
		noticeBox = sharedEle.Find("Notice").gameObject;
		TitleLabel = sharedEle.Find("MenuTitle").transform.Find("Label").GetComponent<UILabel>();
		InfoLabel = sharedEle.Find("Information Box").transform.Find("Scroll View").transform.Find("Item Properties").GetComponent<UILabel>();
		InstructLabel = sharedEle.Find("Instructions").GetComponent<UILabel>();

		base.Initialise ();
		townScreen = parentScreen as UITownScreen;
	}

	public override void Update()
	{
		base.Update();

		if (flipState > 0)
		{
			ProcessFlip();
		}
		
		if (confirmState > 0)
		{
			ProcessConfirmBox();
		}
		
		if (noticeState > 0)
		{
			ProcessNoticeBox();
		}
	}

	public override void OnEnable()
	{
		//if (activePanel != null)
	}
	
	public void UpdatePointer (float angle)
	{		
		if (!pointerTransform.gameObject.activeInHierarchy) return;
		
		pointerTransform.rotation = Quaternion.Euler(0f,0f,angle);
	}

	/// <summary> Return item to inventory if space permits. </summary>
	public bool Unequip(int slot)
	{
		return true;
	}

	public void RequestTransitionToPanel(int index)
	{
		transitionTarget = index;
		flipState = 1;
	}

	public override void TransitionToPanel(int index)
	{	
		activePanel.gameObject.SetActive(false);
		activePanel = panels[index];
		activePanel.gameObject.SetActive(true);

		if (index == 2)
		{
			(parentScreen as UITownScreen).Ready(true);
		}
	}

	public override void AddAllMenuPanels()
	{
		panels[(int)EBackpackPanels.BACKPACK] = transform.FindChild("BackpackMenu").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.TOWN] = transform.FindChild("TownMenu").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.TOWER] = transform.FindChild("TowerConfirm").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.ACCSHOP] = transform.FindChild("AccessoryShop").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.CONSHOP] = transform.FindChild("ConsumableShop").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.SKILLS] = transform.FindChild("Skills").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.TAVERN] = transform.FindChild("Tavern").GetComponent<UIPlayerMenuPanel>();
		panels[(int)EBackpackPanels.CHAPEL] = transform.FindChild("Chapel").GetComponent<UIPlayerMenuPanel>();

		for (int i = 0; i < panels.Count; ++i)
		{
			panels[i].SetParent(this);
			panels[i].gameObject.SetActive(true);
			panels[i].Initialise();
			panels[i].gameObject.SetActive(false);
		}
		
		activePanel = panels[(int)EBackpackPanels.TAVERN];
		//player.ActivePlayerPanel = activePanel;
		NGUITools.SetActive(activePanel.gameObject,true);
		activePanel.OnEnable();
	}

	/// <summary>
	/// Initialize all panels with the values of the player.
	/// </summary>
	public void PanelPlayerSetup()
	{

	}

	public void ShowArrow(bool state)
	{
		GameObject temp = pointerTransform.gameObject;
		NGUITools.SetActive(temp , state);
	}

	/// <summary>
	/// Toggles visibility
	/// </summary>
	public void ShowArrow()
	{
		ShowArrow(!pointerTransform.gameObject.activeSelf);
	}

	public void ShowSharedElements(bool yayOrNay)
	{
		NGUITools.SetActive(sharedEle.gameObject, yayOrNay);
	}

	public void ShowInfo(bool state)
	{
		GameObject temp = sharedEle.FindChild("Information Box").gameObject;
		NGUITools.SetActive(temp , state);
	}

	public void SetTitle(string replace)
	{
		TitleLabel.text = replace;
	}
	
	public void SetInfo(string replace)
	{
		InfoLabel.text = replace;
	}

	public void SetInstructions(string replace)
	{
		InstructLabel.text = replace;
	}
	
	public void RequestConfirmBox(string str)
	{
		confirmBox.GetComponentInChildren<UILabel>().text = str + "\n\n Press (A) to confirm, or (B) to cancel";
		
		confirmState = 1;
	}

	public void RequestNoticeBox(string str)
	{
		noticeBox.GetComponentInChildren<UILabel>().text = str + "\n\n Press (A) to continue";
		
		noticeState = 1;
	}

	#region Processes
	public void ProcessConfirmBox()
	{
		switch (confirmState)
		{
		case 0:
			Debug.LogError("confirmState = 0. Should never happen.");
			break;
		case 1:
			++confirmState;
			//NGUITools.SetActive(confirmBox, true);
			confirmBox.GetComponent<UIPlayTween>().Play(true);
			break;
		case 2:
			if (confirmBox.GetComponent<UITweener>().tweenFactor >= 1f)
			{				
				++confirmState;
			}
			break;
		case 3:
		{
			if (player.Input.A.IsPressed)
			{
				confirmBox.GetComponent<UIPlayTween>().Play(false);
				++confirmState;
				PopupClose.Invoke(true);
			}
			else if (player.Input.B.IsPressed)
			{
				confirmBox.GetComponent<UIPlayTween>().Play(false);
				++confirmState;
				PopupClose.Invoke(false);
			}
		}
			break;
		case 4:
			if (confirmBox.GetComponent<UITweener>().tweenFactor <= 0f)
			{				
				confirmState = 0;
				//NGUITools.SetActive(confirmBox, false);
			}
			break;
		}
	}

	public void ProcessNoticeBox()
	{
		switch (noticeState)
		{
		case 0:
			Debug.LogError("warningState = 0. Should never happen.");
			break;
		case 1:
			++noticeState;
			//NGUITools.SetActive(confirmBox, true);
			noticeBox.GetComponent<UIPlayTween>().Play(true);
			break;
		case 2:
			if (noticeBox.GetComponent<UITweener>().tweenFactor >= 1f)
			{				
				++noticeState;
			}
			break;
		case 3:
		{
			if (player.Input.A.IsPressed)
			{
				noticeBox.GetComponent<UIPlayTween>().Play(false);
				++noticeState;
				PopupClose.Invoke(true);
			}
			else if (player.Input.B.IsPressed)
			{
				noticeBox.GetComponent<UIPlayTween>().Play(false);
				++noticeState;
				PopupClose.Invoke(false);
			}
		}
			break;
		case 4:
			if (noticeBox.GetComponent<UITweener>().tweenFactor <= 0f)
			{				
				noticeState = 0;
				//NGUITools.SetActive(confirmBox, false);
			}
			break;
		}
	}

	protected void ProcessFlip()
	{
		switch (flipState)
		{
		case 0:
			Debug.LogError("flipState = 0. Should never happen.");
			break;
		case 1:
			flipState = 2;
			spinScript.ElapsedSeconds = 0f;
			spinScript.enabled = true;
			break;
		case 2:
			if (spinScript.ElapsedSeconds <= 0.25f) return;

			flipState = 3;
			NGUITools.SetActive(cardBack, true);
			TransitionToPanel(transitionTarget);

			break;
		case 3:
			if (spinScript.ElapsedSeconds <= 0.75f) return;
			
			flipState = 4;
			NGUITools.SetActive(cardBack, false);
			break;
		case 4:
			if (spinScript.ElapsedSeconds <= 0.95f) return;
			
			flipState = 5;
			spinScript.enabled = false;
			break;
		case 5:
			flipState = 0;
			transform.rotation = Quaternion.Euler(Vector3.zero);
			break;
		}
	}
	#endregion

	protected override void HandleInputEvents()
	{
		// disallow input while flip animation is playing
		if (PopupActive) return;

		base.HandleInputEvents();
	}

	public void RequestQuit()
	{
		townScreen.RequestQuit();
	}

	public override void ReadyWindow (bool ready)
	{
		base.ReadyWindow (ready);
	}

	void OnDestroy()
	{
	}
}

