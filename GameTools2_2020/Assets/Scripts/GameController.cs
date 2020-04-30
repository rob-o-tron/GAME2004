using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

namespace GRIDCITY
{
    public class GameController : MonoBehaviour
    {

        #region Fields
        public enum GameState
        {
            Title,
            Prompt,
            Game,
            GameOver,
            Congrats
        };
        private GameState _gameState = GameState.Title;

        private static GameController _instance;
        public GridCityManager cityManager;
        public AudioMaster audioMaster;
        public Camera mainCamera;
        public LayerMask[] layerMaskArray;
        private int whichLayerMask=0;
        public Transform pressKeyObject;
        public Transform gameOverObject;
        public Transform congratsObject;

        //stuff for fading overlay between title and in-game
        public CanvasGroup TitleOverlay;
        public CanvasGroup GameOverlay;
        public AnimationCurve fadeCurve;

        //stuff for intro camera moves
        public Transform cinematicPivot;
        public Transform dummyPivot;
        public PlayableDirector cinematicTimeline;

        //stuff for Player
        public Transform playerPrefab;
        public Transform player;
        public Transform playerBody;
        [SerializeField]
        private int maxHealth = 100;
        private int playerHealth;
        private float playerDamageInterval = 0.1f;
        private float damageTimer=0.1f;
        public static int playerLoot = 0;

        //stuff for game
        public Transform pointPrefab;
        public static int maxLoot = 0;
        public TextMeshProUGUI scoreText;
        public Image healthBarImg;
        public static bool patrolTargetsNeeded = true;
        [SerializeField]
        private int maxPatrolTargets=20;
        private List<Transform> patrolLocations = new List<Transform>();
        public Transform targetPrefab;
        [SerializeField]
        private int maxBaddies = 5;
        private int baddiesRegistered = 0;
        public static bool baddiesNeeded = false;
        public Transform baddiePrefab;
        //private 

        //Environment
        public Material[] skyMatArray;
        public Material[] floorMatArray;
        public Renderer floorRenderer;
        public int previousSkyIndex = 0;
        public int previousFloorIndex = 0;
        public BuildingProfile[] profileLibraryArray;
        
        #endregion

        #region Properties	
        public static GameController Instance
        {
            get
            {
                return _instance;
            }
        }

        public GameState gameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                GameState prevState = _gameState;
                _gameState = value;
                if (prevState != _gameState)
                {
                    Debug.Log("GameController entered " + value.ToString() + " state.");
                    switch (value)
                    {
                        case GameState.Title:
                            ResetGame();
                            break;
                        case GameState.Prompt:
                            pressKeyObject.gameObject.SetActive(true);
                            break;
                        case GameState.GameOver:
                            gameOverObject.gameObject.SetActive(true);
                            PlayLoseFX();
                            break;
                        case GameState.Congrats:
                            congratsObject.gameObject.SetActive(true);
                            PlayWinFX();
                            break;
                        default:
                            break;
                    }
                       
                }
            }
        }

        #endregion

        #region Methods
        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            //randomize seed
            Random.InitState(System.Environment.TickCount);
            //Start our game loop coroutine:
            StartCoroutine(GameLoop());
        }

        private void Awake()
        {
            patrolLocations.Clear();
              
            if (_instance == null)
            {
                _instance = this;
            }

            else
            {
                Destroy(gameObject);
                Debug.LogError("Multiple GameController instances in Scene. Destroying clone!");
            };
        }

        void Update()
        {
            switch (gameState)
            {
                case GameState.Title: //during "intro" we don't respond to input
                    break;
                case GameState.Prompt:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        gameState = GameState.Title;
                    };
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        whichLayerMask = (whichLayerMask + 1) % layerMaskArray.Length;
                        mainCamera.cullingMask = layerMaskArray[whichLayerMask];
                    }
                    else if (Input.anyKeyDown)
                    {
                        gameState = GameState.Game;
                    };
                    break;
                case GameState.Game:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        gameState = GameState.GameOver;
                    };
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        whichLayerMask = (whichLayerMask + 1) % layerMaskArray.Length;
                        mainCamera.cullingMask = layerMaskArray[whichLayerMask];
                    };
                    break;
                case GameState.GameOver:
                case GameState.Congrats:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        gameState = GameState.Title;
                    };
                    break;
                default:
                    break;
                
            }
        }

        #endregion

        IEnumerator GameLoop()
        {
            //We start with Game State "Title" (see _gameState initialization)
            Random.InitState(System.Environment.TickCount);
            //Reset what needs to be reset
            maxLoot = 0;
            playerLoot = 0;
            baddiesRegistered = 0;
            patrolLocations.Clear();
            patrolTargetsNeeded = true;
            cityManager.SetNavMeshReadyFlag(false);
            audioMaster.Reset();

            audioMaster.PlayTitles();
            cinematicTimeline.Play();

            //Randomize game environment for a new game:
            RandomizeStage();
            RandomizeTowerFlavours();
            yield return new WaitForSeconds(2f);
            cityManager.BuildTowers(Random.Range(25,45));
            yield return new WaitForSeconds(2f);
            cityManager.BuildRoads();
            yield return new WaitForSeconds(3f);
            cityManager.BakeNavMesh();
            yield return new WaitForSeconds(3f);
            cityManager.SetNavMeshReadyFlag(true);
            yield return new WaitForSeconds(2.0f);

            gameState = GameState.Prompt;

            while (gameState==GameState.Prompt) //just wait and yield execution to rendering
            {
                yield return null;
            }
            // we must be entering game now, so do a few things:
            //1. Spawn Player
            player = Instantiate(playerPrefab, cityManager.startLocation.position,Quaternion.identity);
            playerHealth = maxHealth;
            playerBody = player.GetComponent<Character>().bodyTransform;
            //2. Switch camera to Player-centric
            mainCamera.transform.SetParent(dummyPivot, true);
            CameraFollow simpleFollow = mainCamera.GetComponent<CameraFollow>();
            simpleFollow.SetTargets(player.GetComponent<Character>().cameraTarget, player.GetComponent<Character>().cameramanTransform);
            //3. redraw UI
            UpdateHealthUI();
            UpdateScoreUI();
            //4. start spawning Baddies:
            baddiesNeeded = true;

            cinematicTimeline.Stop();
            DoOverlayTransitionFade(2f);
            audioMaster.FadeOut(2f);
            yield return new WaitForSeconds(2f);
            audioMaster.Reset();
            audioMaster.FadeIn(0f);
            audioMaster.PlayIngame();

            while (gameState == GameState.Game) //main game loop
                                               //core game logic (initialize, detect win/lose)
            {
                damageTimer += Time.deltaTime;
                //check for death
                if (playerHealth < 0) gameState = GameState.GameOver;
                if (playerLoot >= maxLoot) gameState = GameState.Congrats;
                yield return null;
            }

            //we either won or lost so wait a little bit to show the end card
            yield return new WaitForSeconds(10f);
            //...and reset the game (reload scene)
            cityManager.ResetArray();
            gameState = GameState.Title;

        }
        /// <summary>
        /// at certain points we need to make sure to reset the entire game state
        /// The easiest way to release all objects and clean up is... to reload the scene
        /// </summary>
        public void ResetGame()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        public void RandomizeStage()
        {
            Material newSkyMaterial;
            bool skySuccess = false;
            while (!skySuccess) //keep 'throwing the die' until sky index is different than the previous one 
            {
                int skyIndex = Random.Range(0, skyMatArray.Length);
                if (skyIndex != previousSkyIndex)
                {
                    newSkyMaterial = skyMatArray[skyIndex];
                    previousSkyIndex = skyIndex;
                    RenderSettings.skybox = newSkyMaterial;
                    skySuccess = true; //we can exit the while loop now
                }
            }

            Material newFloorMaterial;
            bool floorSuccess = false;
            while (!floorSuccess) //keep 'throwing the die' until floor index is different than the previous one 
            {
                int floorIndex = Random.Range(0, floorMatArray.Length);
                if (floorIndex != previousFloorIndex)
                {
                    newFloorMaterial = floorMatArray[floorIndex];
                    previousFloorIndex = floorIndex;
                    floorRenderer.material = newFloorMaterial;
                    floorSuccess = true; //we can exit the while loop now
                }
            }
        }

        //for each game, select a subset from GameController's tower profile library 
        //and assign it to GridCityManager's gameProfileArray
        public void RandomizeTowerFlavours()
        {
            int arraySize = Random.Range(1, profileLibraryArray.Length);
            BuildingProfile[] profileArray = new BuildingProfile[arraySize];
            for (int i=0; i<arraySize; i++)
            {
                int rnd = Random.Range(0, profileLibraryArray.Length);
                profileArray[i] = profileLibraryArray[rnd];
            }
            cityManager.gameProfileArray = profileArray;
        }

        public void DoOverlayTransitionFade(float duration)
        {
            StartCoroutine(OverlayTransition(duration));
        }

        public void HurtPlayer()
        {
            if (gameState==GameState.Game)
            {
                if (damageTimer>playerDamageInterval)
                {
                    playerHealth--;
                    UpdateHealthUI();
                    PlayHurtFX();
                    damageTimer = 0.0f;
                }
            }

        }

        public void UpdateHealthUI()
        {
            healthBarImg.fillAmount = (float)playerHealth / (float)maxHealth;
        }

        public void UpdateScoreUI()
        {
            scoreText.text = playerLoot.ToString() + "/" + maxLoot.ToString();
        }

        public void RegisterBaddie()
        {
            baddiesRegistered++;
            if (baddiesRegistered>=maxBaddies)
            {
                baddiesNeeded = false; //tiles to stop spawning baddies
            };
        }

        public void RegisterPatrolTarget(Transform target)
        {
            patrolLocations.Add(target);
            //Debug.Log("Registered: " + patrolLocations.Count.ToString());
            if (patrolLocations.Count>=maxPatrolTargets)
            {
                patrolTargetsNeeded = false; //tiles to stop spawning patrol targets
            };
        }

        public Transform RequestPatrolTarget()
        {
            int patrolCount = patrolLocations.Count;
            if (patrolCount > 0)
                return (patrolLocations[Random.Range(0, patrolCount)]);
            else
                return null;
        }

        public void CollectPoint()
        {
            playerLoot++;
            UpdateScoreUI();
            PlayCoinFX();
        }

        public void PlayCoinFX()
        {
            audioMaster.PlayFX(0);
        }
        public void PlayWinFX()
        {
            audioMaster.PlayFX(1);
        }

        public void PlayHurtFX()
        {
            audioMaster.PlayFX(2);
        }

        public void PlayLoseFX()
        {
            audioMaster.PlayFX(3);
        }

        IEnumerator OverlayTransition(float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            TitleOverlay.alpha = 1.0f;
            GameOverlay.alpha = 0.0f;
            yield return null;
            while (Time.time<endTime)
            {
                float eval = (Time.time - startTime)/duration;
                float currentalpha = fadeCurve.Evaluate(eval);
                GameOverlay.alpha = currentalpha;
                TitleOverlay.alpha = 1.0f - currentalpha;
                yield return null;
            }
            GameOverlay.alpha = 1.0f;
            TitleOverlay.alpha = 0.0f;
        }
        #endregion


    }

}
