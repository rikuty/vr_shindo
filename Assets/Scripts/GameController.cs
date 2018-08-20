﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameController : UtilComponent {


	//public CountDownComponent cdCountDown;

    private Context _context;
    public Context context{
        get{
            if(this._context == null){
                this._context = new Context();
            }
            return this._context;
        }
    }


	public enum STATUS_ENUM : int{
		START,
		COUNT,
		PLAY,
        FINISH,
        SHOW_RESLUT
	}
    private STATUS_ENUM currentStatus = STATUS_ENUM.START;

    [SerializeField] private EventController eventController;

    [SerializeField] private CountDownComponent cdComponent;

    [SerializeField] private Transform trStart;
    [SerializeField] private GameObject objCountDown;
    [SerializeField] private GameObject objPlay;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AnswerObjectController answerController;
    [SerializeField] private Text curretAnswer;
    [SerializeField] private Text numMinus;
    [SerializeField] private Text correctCount;

    [SerializeField] private SordCotroller sordCotroller;


    [SerializeField] private Text x;
    [SerializeField] private Text y;
    [SerializeField] private Text z;

    [SerializeField] private ResultModalPresenter resultModal;

    private StartObject startObject;

	// Use this for initialization
	private void Start () {
        this.currentStatus = STATUS_ENUM.START;

        this.eventController.Init(this.CallbackCut, this.context);
        this.answerController.Init(this.context);
        this.sordCotroller.Init(this.context);

        startObject = ResourceLoader.Instance.Create<StartObject>("Prefabs/CubeStart", trStart);
        startObject.Init();
        SetActive(this.trStart, true);
        SetActive(this.objCountDown, false);
        SetActive(this.objPlay, false);

	}

    private void CallbackCut(string objName){
        if(objName == "CubeStart"){
            this.currentStatus = STATUS_ENUM.COUNT;
            //Debug.Log("CubeStart");
            this.startObject.WasCut();
            StartCoroutine(this.SetCountDown());
        }else {
            if (this.currentStatus != STATUS_ENUM.PLAY) return;
           this.answerController.Answer(objName);
        }
    }


    private IEnumerator SetCountDown(){
        //Debug.Log("CountDown");

        yield return new WaitForSeconds(2);

        this.cdComponent.Init(3.0f, this.FinishCountDown, false);
        SetActive(this.trStart, false);
        SetActive(this.objCountDown, true);
        SetActive(this.objPlay, false);
        SetActive(this.resultModal.gameObject, false);
    }
	
	// Update is called once per frame
	private void Update () {
        OVRInput.Controller activeController = OVRInput.GetActiveController();
        Quaternion rot = OVRInput.GetLocalControllerRotation(activeController);
        SetLabel(this.x, rot.eulerAngles.x.ToString());
        SetLabel(this.y, rot.eulerAngles.y.ToString());
        SetLabel(this.z, rot.eulerAngles.z.ToString());
        //Debug.Log("x:" + this.x.text);
        //Debug.Log("y:" + this.y.text);
        //Debug.Log("z:" + this.z.text);
        switch(this.currentStatus){
            case STATUS_ENUM.START:
                this.UpdateStart();
    			break;
    		case STATUS_ENUM.COUNT:
    			//this.UpdateCount();
    			break;
    		case STATUS_ENUM.PLAY:
    			this.UpdatePlay();
    			break;
            case STATUS_ENUM.FINISH:
                this.UpdateFinish();
                break;
            case STATUS_ENUM.SHOW_RESLUT:
                this.UpdateShowResult();
                break;
		}
	}
	

	private void UpdateStart(){
	}


	private void FinishCountDown(){
		//this.cdCountDown.Init(3f, ()=>
        //{
        this.currentStatus = STATUS_ENUM.PLAY;
        this.context.Init();
        this.context.StartPlay();
        //this.answerController.Init(this.context);
        this.answerController.SetAnswers();


        SetActive(this.trStart, false);
        SetActive(this.objCountDown, false);
        SetActive(this.objPlay, true);
        //if (this.audioSource != null)
        //{
        //    this.audioSource.PlayOneShot(this.audioClip);
        //}

		//},
		//true);

	}

	private void UpdatePlay(){
        if(!this.context.isPlay){
            this.currentStatus = STATUS_ENUM.FINISH;
            return;
        }
        SetLabel(this.curretAnswer, this.context.currentAnswer.ToString());
        SetLabel(this.numMinus, this.context.numMinus.ToString());
        SetLabel(this.correctCount, this.context.correctCount.ToString());
        //this.context.SetLeftTime(Time.deltaTime);
	}

    private void UpdateFinish()
    {
        this.context.WatchStop();
        ResultModalModel model = 
            new ResultModalModel(this.context.correctCount, this.context.quizCount, this.context.playTime);
        resultModal.Show(model);
        this.currentStatus = STATUS_ENUM.SHOW_RESLUT;
    }

    private void UpdateShowResult(){
        
    }


    private void ClickedFinish()
    {
        Gamestrap.GSAppExampleControl.Instance.LoadScene(Gamestrap.ESceneNames.scene_title);

    }


}
