using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int PlayerTurn = 1;

    [Header("P1")]
    public int PointPlayer1 = 0;
    public Text P1_text;
    public GameObject P1_Arrow;


    [Header("P2")]
    public int PointPlayer2 = 0;
    public Text P2_text;
    public GameObject P2_Arrow;



    [Header("Canvas")]
    public GameObject GameCanvas;
    public GameObject EndCanvas;
    public GameObject PauseCanvas;

    private int Pwin; 
    public Text PwinText;

    [Header("Grid and Cards")]
    public GameObject pref_Card;
    public Vector2 Grid = new Vector2(5, 4);
    public float OffsetX, OffsetZ;
    public List<Card> AllCards = new List<Card>();
    public List<Card> selectedCards = new List<Card>();


    private Ray _rayCard;
    private RaycastHit _hitCard;

    private bool turningCards = false;
    private bool GamePaused = false;


    // Use this for initialization
    void Start()
    {

        Time.timeScale = 1;
        InitCards();

        GameCanvas.SetActive(true);
        PauseCanvas.SetActive(false);
        EndCanvas.SetActive(false);

        P1_text.text = "Player 1:  " + PointPlayer1;
        P2_text.text = "Player 2:  " + PointPlayer2;
    }


    private void InitCards()
    {
        float impares = (Grid.x * Grid.y) % 2;

        if (impares != 0)
            Grid.y = Grid.y - 1 == 0 ? Grid.y + 1 : Grid.y - 1;


        for (int x = 0; x < Grid.x; x++)
        {
            for (int z = 0; z < Grid.y; z++)
            {
                GameObject newCard = Instantiate(pref_Card, new Vector3(x * OffsetX - 1.2f, 1.9f, z * OffsetZ - 0.87f), Quaternion.Euler(-90, 180, 0));
                int tempID = Random.Range(0, 10);

                while (CheckRepeatedCards(tempID))
                {
                    tempID = Random.Range(0, 10);
                }
                AllCards.Add(new Card(newCard, tempID));
            }
        }
    }

    private bool CheckRepeatedCards(int index)
    {
        int contador = 0;

        for (int i = 0; i < AllCards.Count; i++)
        {
            if (AllCards[i].ID == index)
            {
                contador++;
            }
        }

        return (contador == 2);
        #region is the same as
        //if (contador == 2)
        //    return false;
        //else
        //    return true;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
                GamePaused = true;
                PauseCanvas.SetActive(true);
                GameCanvas.SetActive(false);
            }
            else
            {
                Time.timeScale = 1;
                GamePaused = false;
                PauseCanvas.SetActive(false);
                GameCanvas.SetActive(true);
            }
        }
        #endregion

        #region Interaction with cards
        if (GamePaused == false)
        {
            if (turningCards == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _rayCard = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(_rayCard, out _hitCard))
                    {
                        if (_hitCard.collider.tag == "Card") //Si colisiona
                        {
                            _hitCard.collider.transform.rotation = Quaternion.Euler(90, 180, 0); //Rotamos su posicion para que se vea

                            _hitCard.collider.enabled = false; // Desactivamos su collider para evitar repetir y eliminar el mismo

                            selectedCards.Add(GetCardByGameObject(_hitCard.collider.transform.gameObject)); //Add item al seleccionar

                            if (selectedCards.Count == 2)
                            {
                                if (selectedCards[0].ID == selectedCards[1].ID) //Cards DO match
                                {
                                    if (PlayerTurn == 1)
                                    {
                                        PointPlayer1++;
                                        P1_text.text = "Player 1:  " + PointPlayer1;
                                    }
                                    else
                                    {
                                        PointPlayer2++;
                                        P2_text.text = "Player 2:  " + PointPlayer2;
                                    }

                                    StartCoroutine(wait4S(1));
                                }    // Cards DO match
                                else                                            //Cards DONT match
                                {
                                    Invoke("TurnCard", 1f);
                                    turningCards = true;

                                    if (PlayerTurn == 1)
                                        PlayerTurn = 2;

                                    else PlayerTurn = 1;
                                }                                              
                            }
                        }
                    }
                }
            }
        }
        #endregion

        if (AllCards.Count <= 0)
            PlayerTurn = 3;

        if(PointPlayer1 + PointPlayer2 == 10)
        {
            if (PointPlayer1 > PointPlayer2)
                Pwin = 1;
            else Pwin = 2;

            PlayerTurn = 3;
        }

        #region Switch Stances
        switch (PlayerTurn)
        {
            case 1:
                P2_Arrow.SetActive(false);
                P1_Arrow.SetActive(true);
                break;
            case 2:
                P1_Arrow.SetActive(false);
                P2_Arrow.SetActive(true);
                break;
            case 3:
                EndCanvas.SetActive(true);
                GameCanvas.SetActive(false);

                if(Pwin == 1)
                    PwinText.text = ("Player 1 wins! ^o^/");
                else PwinText.text = ("Player 2 wins! ^o^/");


                break;
            default:
                break;
        }

        #endregion

    }

    IEnumerator wait4S(int time)
    {
        yield return new WaitForSeconds(time);
        Destroy(selectedCards[0].Piece);
        Destroy(selectedCards[1].Piece);
        selectedCards.Clear();
    }

    private void TurnCard()
    {
        selectedCards[0].Piece.transform.rotation = Quaternion.Euler(-90, 180, 0);
        selectedCards[1].Piece.transform.rotation = Quaternion.Euler(-90, 180, 0);
        selectedCards[0].Piece.GetComponent<BoxCollider>().enabled = true;
        selectedCards[1].Piece.GetComponent<BoxCollider>().enabled = true;

        selectedCards.Clear();

        turningCards = false;
    }
    private Card GetCardByGameObject(GameObject obj)
    {
        for (int i = 0; i < AllCards.Count; i++)
        {
            if (obj == AllCards[i].Piece)
                return AllCards[i];
        }

        return null;
    }


    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void ResumeGame()
    {
        PauseCanvas.SetActive(false);
        GameCanvas.SetActive(true);
        Time.timeScale = 1;
        GamePaused = false;
    }
    public void RestartGame()
    {
        if (Time.timeScale != 1)
        {
            GamePaused = false;
            Time.timeScale = 1;
        }

        for (int i = 0; i < AllCards.Count; i++)
        {
            Destroy(AllCards[i].Piece);
        }
        PointPlayer1 = 0;
        P1_text.text = "Player 1:  " + PointPlayer1;
        PointPlayer2 = 0;
        P2_text.text = "Player 2:  " + PointPlayer2;

        PlayerTurn = 1;
        AllCards.Clear();
        ResumeGame();
        InitCards();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
