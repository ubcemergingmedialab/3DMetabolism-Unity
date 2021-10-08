using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  NOTE: UPDATE IF FUNCTIONALITY CHANGES
    2021-10-07 
        This Script is a Centralized point of access for Any Status within the program. This includes:
        Active pathways, pathway Highlight states, favourite pathways etc.
        currently getting optimized for Handeling highlights. functions/fields related to highlighting to be added:
            - Calc max/ min highlight state -> possibly in HighlightService
            done:
            - node/edge Status lists
            - ElementCheckState
            - PAthwaySOCheckState
            - list of active PAthways
            - intialize a highlightPathway per pathwaySO ( dict<PWSO, HighlightPAthway>)

*/
public class StatusController : MonoBehaviour
{
    //SINGELTON
    private static StatusController _instance;
    public static StatusController Instance
    {
        get { return _instance; }
    }

    //fields
    // GameObject UIContainer;
    private Dictionary<HighlightHandler, List<HighlightPathway>> elementToPathways; // <> key = nodes/edges , entry = list of pathways connected to it
    private Dictionary<PathwaySO, HighlightPathway> highlightByPathwaySO;            // <> Status controller intializes and connects each highlitpathway to the pathwaySOs
    private List<HighlightPathway> highlightPathways;                               // <> to keep track of all highlightPAthways initialized

    public List<PathwaySO> activePathways;                                      // <> to be filled manually in unity 

    void Awake() 
    {
        if (_instance != null && _instance != this) 
            {
                Destroy(this.gameObject);
                return;
            }
        _instance = this;   
        DontDestroyOnLoad(this.gameObject);


        elementToPathways = new Dictionary<HighlightHandler, List<HighlightPathway>>();
        highlightByPathwaySO = new Dictionary<PathwaySO, HighlightPathway>();

        // <> fill the elements network 
        foreach (PathwaySO pathwaySO in activePathways) {
            
            HighlightPathway highlightPathway = new HighlightPathway(pathwaySO);                                    // <> initialize a highlightPathway per active pathway
            highlightByPathwaySO.Add(pathwaySO,highlightPathway);    // link the pathwaySO to its highlightPathway
            highlightPathways.Add(highlightPathway);                                                                // add the new highlight pathway to the list that keeps track of them

            foreach(NodeSO nodeSO in pathwaySO.nodes) {                                                             // For every node in this pathway
                GameObject[] nodes = GameObject.FindGameObjectsWithTag(nodeSO.name);

                foreach(GameObject node in nodes) {
                    if(node != null) {
                        HighlightHandler hl = node.GetComponent<HighlightHandler>();                                // find the nodes handler
                        List<HighlightPathway> sharingPathways;                                                     

                        if(elementToPathways.TryGetValue(hl, out sharingPathways)) {                                // Find node in elementToPathways
                            sharingPathways.Add(highlightPathway);                                                  // add the highlightPathway to the element's list of hpw
                        } else {
                            elementToPathways.Add(hl, new List<HighlightPathway>{highlightPathway});                // Make a new list if node is not yet in the Dictionary 
                        }
                    }
                }
            }

            foreach(EdgeSO edgeSO in pathwaySO.edges) {                                                             // For every edge in this pathway (same proccess as nodes)
                GameObject[] edges = GameObject.FindGameObjectsWithTag(edgeSO.name);

                foreach(GameObject edge in edges){
                    if(edge != null) {
                        HighlightHandler hl = edge.GetComponent<HighlightHandler>();
                        List<HighlightPathway> sharingPathways; 

                        if(elementToPathways.TryGetValue(hl, out sharingPathways)) {                  
                            sharingPathways.Add(highlightPathway);
                        } else {
                            elementToPathways.Add(hl, new List<HighlightPathway>{highlightPathway});      
                        }
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HighlightPathway.HighlightState ElementCheckState(HighlightHandler highlightHandler) {
        
        HighlightPathway.HighlightState tempState = HighlightPathway.HighlightState.Default;
        elementToPathways.TryGetValue(highlightHandler, out List<HighlightPathway> currentList);

        if ( currentList != null) {
            foreach ( HighlightPathway hlpw in currentList) {
                HighlightPathway.HighlightState newState = hlpw.state;
                    if ( newState > tempState) {
                        tempState = newState;
                    }
            }
        } else {
            Debug.LogError("no pathwaylist are to be found on the elementToPathways Dictionary (NULL access)");
        }
        return tempState;
    }

// <> returns the current state of a given pathway through its SO
    public HighlightPathway.HighlightState PathwayCheckState(PathwaySO pathway) {
        HighlightPathway.HighlightState tempState = HighlightPathway.HighlightState.Default;
        highlightByPathwaySO.TryGetValue(pathway, out HighlightPathway highlightpathway);

        if (highlightpathway != null ){
            tempState = highlightpathway.state;
        } else{
            Debug.LogError("no highlight pathway linked to this PathwaySO");
        }

        return tempState;
    }

    public Dictionary<HighlightHandler, List<HighlightPathway>> GetElementToPathways() {
        return elementToPathways;
    }

}