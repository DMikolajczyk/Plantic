using System.Linq;
using UnityEngine;
using WindowsEnum;

public class TouchFunctions : MonoBehaviour {
    
	void Start () {
        cameraStartPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
	}
    ///////////////// Managers ////////
    public ManagersContainer managers;
    public WindowsManager windowsManager;
    //////////////// camera and touch variables///////////////
	private Vector2 fingerBeganPosition;
    private float speed = 0.02F;
    private bool blockMove = false;
    private Vector3 cameraStartPosition;
        
    void Update()
	{
        if (!windowsManager.isAnyBlockCameraWindowOpen())
        {
            touchAction();
        }
    }
    void FixedUpdate()
    {
        if (!windowsManager.isAnyBlockCameraWindowOpen())
        {
            moveCamera();
        }
    }

	private void touchAction() {
		if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
			switch(touch.phase) {
			case TouchPhase.Began:
                fingerBeganPosition = Input.GetTouch(0).position;
				break;
			case TouchPhase.Ended:
                float dist = Vector2.Distance(fingerBeganPosition, Input.GetTouch(0).position);
                if ((dist < 10) && !Utilities.isAfterButtonClick)
                {
                    Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(raycast, out raycastHit))
                    {
                        GameObject touchedObject = raycastHit.collider.gameObject;
                        ////////// Kliknięcie gdziekolwiek zamyka okienka od rośliny
                        if(windowsManager.isOpen((int)Windows.PlantManager))
                        {
                            windowsManager.close((int)Windows.PlantManager);
                        }
                        ////////// Kliknięcie drugiej rośliny podczas krzyżowania
                        else if ((managers.plantManager.CrossMode == true) && (touchedObject.CompareTag("plant") || touchedObject.CompareTag("plantRoot")))
                        {
                            managers.plantManager.dnaTwoToCross = null;
                            int[] dnaOfTouched;
                                    
                            ///////// /ustawienie dna dotkniętego planta lub jego rodzica.
                            if(touchedObject.transform.parent != null)
                            {
                                dnaOfTouched = managers.plantManager.getDna(touchedObject.transform.root.transform.position.x, touchedObject.transform.root.transform.position.z);
                            }
                            else
                            {
                                dnaOfTouched = managers.plantManager.getDna(touchedObject.transform.position.x, touchedObject.transform.position.z);
                            }
                                    
                            ///////// Jeśli klikniemy roślinę z innym dna niż pierwszej rośliny to ustaw drugie DNA.
                            if(!Enumerable.SequenceEqual(dnaOfTouched, managers.plantManager.dnaOneToCross))
                            {
                                managers.plantManager.dnaTwoToCross = dnaOfTouched;
                            }
                            ///////// Po kliknięciu drugiej rośliny krzyżuj
                            if (managers.plantManager.dnaTwoToCross != null)
                            {
                                windowsManager.close((int)Windows.InfoCrossing);
                                windowsManager.open((int)Windows.Crossing);
                            }
                                    
                        }
                        //////// Kliknięcie planea czyli otworzenie plecaka.
                        else if ((managers.plantManager.CrossMode == false) && touchedObject.CompareTag("planeClickable") && (!windowsManager.isAnyInfoDialogOpen()))
                        {
                            Vector3 clickPlanePosition = new Vector3(roundCoord(raycastHit.point.x), 0, roundCoord(raycastHit.point.z));
                            if (managers.planesManager.isFree(new Vector2(clickPlanePosition.x, clickPlanePosition.z)))
                            {
                                windowsManager.openBag(clickPlanePosition);
                            }

                        }
                        //////// Kliknięcie pierwszej rośliny
                        else if ((managers.plantManager.CrossMode == false) && (touchedObject.CompareTag("plant") || touchedObject.CompareTag("plantRoot")) && (!windowsManager.isAnyInfoDialogOpen()))
                        {
                            windowsManager.openPlantManager(touchedObject, raycastHit.point);
                        }
                        //////// Kliknięcie szczątek
                        else if ((managers.plantManager.CrossMode == false) && (touchedObject.CompareTag("garbage")) && (!windowsManager.isAnyInfoDialogOpen())) {
                            windowsManager.openBag(touchedObject.transform.position, touchedObject);
                        }
                    }
                }
                else
                {
                    Utilities.isAfterButtonClick = false;
                } 
				break;
			}

		}

	}

    public void moveCamera()
    {
        blockMove = false;
        if (transform.localPosition.x > cameraStartPosition.x + ((Utilities.fieldSize * Utilities.scaleUnit) / 1.5))
        {
            transform.Translate(0.1F, 0, 0);
            blockMove = true;
        }
        if (transform.localPosition.x < cameraStartPosition.x - ((Utilities.fieldSize * Utilities.scaleUnit) / 1.5))
        {
            transform.Translate(-0.1F, 0, 0);
            blockMove = true;
        }
        if (transform.localPosition.y > cameraStartPosition.y + ((Utilities.fieldSize * Utilities.scaleUnit) / 2))
        {
            transform.Translate(0, -0.1F, 0);
            blockMove = true;
        }
        if (transform.localPosition.y < cameraStartPosition.y - ((Utilities.fieldSize * Utilities.scaleUnit) / 2))
        {
            transform.Translate(0, 0.1F, 0);
            blockMove = true;
        }


        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) && (blockMove == false))
        {
            Vector2 moves = Input.GetTouch(0).deltaPosition * speed;
            transform.Translate(-moves.x, -moves.y, 0);

        }
    }

    private float roundCoord(float coord)
    {
        float a = coord % Utilities.scaleUnit;
        float b = 0;
        if (coord == 0)
        {
            b = 0;
        }
        else if (coord > 0)
        {
            if (a < Utilities.scaleUnit / 2)
            {
                b = coord - a;
            }
            else
            {
                b = coord + (Utilities.scaleUnit - a);
            }
        }
        else
        {
            a = Mathf.Abs(a);
            if (a < Utilities.scaleUnit / 2)
            {
                b = coord + a;
            }
            else
            {
                b = coord - (Utilities.scaleUnit - a);
            }
        }
        return b;
    }

}
