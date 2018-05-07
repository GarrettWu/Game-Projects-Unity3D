using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellGenerator : MonoSingleton< CellGenerator >
{
	public GameObject m_tile_root;
	public GameObject m_tile_I1;
	public GameObject m_tile_I2;
	public GameObject m_tile_I3;
	public GameObject m_tile_I4;
	public GameObject m_tile_LCw1;
	public GameObject m_tile_LAcw1;
	public GameObject m_tile_I_broken1;
	public GameObject m_tile_T1;
	
	public List<GameObject> m_cellObjects = new List<GameObject> ();

	private GameObject cellModel_new;
	
	private GameObject cellModel_prev;
	
	public GameObject cellModel_cornerT;
	
	public GameObject prefab_prop_magnet;
	
	
	// The maximum number of cells to be created ahead of the player (NOT the maximum length of the track; this changes dynamically)
	private int	m_maxCells = 8;
	
	//the cell which player in
//	public GameObject m_playerCell = null;
	//the cell which ready to be deleted
	public GameObject m_toDeleteCell = null;
	//
	private int count_tile_L1 = 0;
	private int count_tile_L2 = 0;
	
	
	//cell generate rule, contains cellName, follow's cellName, probability
	private Dictionary<enCellName,Dictionary<enCellName,int>> ruleMap = new Dictionary<enCellName,Dictionary<enCellName,int>>{
	{
			enCellName.I1, 
			new Dictionary<enCellName,int>{
				{enCellName.I1, 15},
				{enCellName.I2, 15},
				{enCellName.I3, 15},
				{enCellName.I4, 15},
				{enCellName.LCw1, 15},
				{enCellName.LAcw1, 15},
				{enCellName.T1, 10},
			}
		},
		
		{
			enCellName.I2,
			new Dictionary<enCellName,int>{
				{enCellName.I1, 15},
				{enCellName.I2, 15},
				{enCellName.I3, 15},
				{enCellName.I4, 15},
				{enCellName.LCw1, 15},
				{enCellName.LAcw1, 15},
				{enCellName.T1, 10},
			}
		},
		
		{
			enCellName.I3,
			new Dictionary<enCellName,int>{
				{enCellName.I1, 15},
				{enCellName.I2, 15},
				{enCellName.I3, 15},
				{enCellName.I4, 15},
				{enCellName.LCw1, 15},
				{enCellName.LAcw1, 15},
				{enCellName.T1, 10},
			}
		},
		
		{
			enCellName.I4,
			new Dictionary<enCellName,int>{
				{enCellName.I1, 15},
				{enCellName.I2, 15},
				{enCellName.I3, 15},
				{enCellName.I4, 15},
				{enCellName.LCw1, 15},
				{enCellName.LAcw1, 15},
				{enCellName.T1, 10},
			}
		},
		
		{
			enCellName.LCw1,
			new Dictionary<enCellName,int>{
				{enCellName.I1, 35},
				{enCellName.I2, 35},
				{enCellName.I3, 15},
				{enCellName.I4, 15},
			}
		},
		
		{
			enCellName.LAcw1,
			new Dictionary<enCellName,int>{
				{enCellName.I1, 35},
				{enCellName.I2, 35},
				{enCellName.I3, 15},
				{enCellName.I4, 15},
			}
		},
		
		{
			enCellName.T1,
			new Dictionary<enCellName,int>{
				{enCellName.I1, 70},
				{enCellName.I2, 30},
			}
		},
	};
	

	
	// Use this for initialization
	void Start ()
	{
		initCells();
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		if(m_cellObjects.Count < m_maxCells)
		{
			createCellList();
		}
		
	}
	
	
	/// <summary>
	/// Inits the cells.
	/// </summary>
	public void initCells()
	{
		cellModel_new = ObjectPool.instance.getObj(m_tile_root, Vector3.zero, Quaternion.identity);
		cellModel_new.transform.parent = this.transform;
		m_cellObjects.Add(cellModel_new);
		
		cellModel_prev = cellModel_new;
		createCellList();
	}
	
	
	/// <summary>
	/// Creates the cell list.
	/// </summary>
	private void createCellList(){
		for(int i=m_cellObjects.Count-1; i<m_maxCells; i++){
		
			GameObject prefabToInstantiate = null;
			prefabToInstantiate = getCellModel(cellModel_prev);
			
			cellModel_new = ObjectPool.instance.getObj(prefabToInstantiate, Vector3.zero, Quaternion.identity);
			CellInfo cellInfo_prev = (CellInfo)cellModel_prev.GetComponent("CellInfo");
				
			cellModel_new.transform.parent = this.transform;
			cellModel_new = setPositionAndRotation(cellModel_prev, cellModel_new);
			
			//todo: delete
//			if(cellModel_new.transform.FindChild("obstacleGroupA")!=null 
//				|| cellModel_new.transform.FindChild("obstacleGroupB")!=null)
//			{
//				int temp = Random.Range(0,10);
//				if(temp<4){
//					cellModel_new.transform.FindChild("obstacleGroupA").gameObject.SetActive(true);
//					if(Random.Range(0,2)==0)
//						cellModel_new.transform.FindChild("coinGroupA").gameObject.SetActive(true);
//				} else if(temp<8){
//					cellModel_new.transform.FindChild("obstacleGroupB").gameObject.SetActive(true);
//					if(Random.Range(0,2)==0)
//						cellModel_new.transform.FindChild("coinGroupB").gameObject.SetActive(true);
//				} else{
//					cellModel_new.transform.FindChild("obstacleGroupC").gameObject.SetActive(true);
//					if(Random.Range(0,2)==0)
//						cellModel_new.transform.FindChild("coinGroupC").gameObject.SetActive(true);
//				}
//			}
			createObstacle(cellModel_new);
			
			m_cellObjects.Add(cellModel_new);
			
			cellModel_prev = cellModel_new;
		}
	}
	
	
	/// <summary>
	/// Gets the cell model.
	/// </summary>
	/// <returns>
	/// The cell model.
	/// </returns>
	/// <param name='prevCell'>
	/// Previous cell.
	/// </param>
	private GameObject getCellModel(GameObject prevCell){
		CellInfo prevCellInfo = (CellInfo)prevCell.GetComponent("CellInfo");
				
		enCellName next_cellName = ruleAnalyse(prevCellInfo.CellName);
		GameObject nextCell = getGameObjByCellName(next_cellName);
		CellInfo nextCellInfo = (CellInfo)nextCell.GetComponent("CellInfo");
		
		//check whether the nextcell is accord with other rules
		switch(nextCellInfo.CellType) {
		case enCellType.corner_clockwise:
			count_tile_L2 = 0;
			if(count_tile_L1==2){
				return getCellModel(prevCell);
			}
			else{
				count_tile_L1++;
			}
			break;
		case enCellType.corner_anticlockwise:
			count_tile_L1 = 0;
			if(count_tile_L2==2){
				return getCellModel(prevCell);
			}
			else{
				count_tile_L2++;
			}
			break;
		case enCellType.corner_T:
			if(cellModel_cornerT!=null)
				return getCellModel(prevCell);
			else {
				count_tile_L1++;
				count_tile_L2++;
				cellModel_cornerT = m_tile_T1;
			}
			break;
		}
		
		if(prevCellInfo.CellType!=enCellType.straight 
			&& nextCellInfo.CellType!=enCellType.straight)
			return getCellModel(prevCell);
		
//		print ("obj: "+nextCell.name+"   name:"+nextCellInfo.CellName);
		return nextCell;
	}
	
	
	/// <summary>
	/// Sets the position and rotation.
	/// </summary>
	/// <returns>
	/// The position and rotation.
	/// </returns>
	/// <param name='prevCell'>
	/// Previous cell.
	/// </param>
	/// <param name='cellModel_new'>
	/// Cell model_new.
	/// </param>
	private GameObject setPositionAndRotation(GameObject prevCell, GameObject cellModel_new)
	{
		CellInfo prevCellInfo = (CellInfo)prevCell.GetComponent("CellInfo");
		CellInfo newCellInfo = (CellInfo)cellModel_new.GetComponent("CellInfo");
		
		Vector3 newCell_Position = prevCell.transform.position;
		Quaternion rotation = Quaternion.identity;

		//Calculate direction and rotation of cell
		switch (newCellInfo.CellType) {
			
			case enCellType.straight:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						rotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.North;
						break;
					case enCellDir.South:
						rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.South;
						break;
					case enCellDir.West:
						rotation = Quaternion.Euler (0.0f, 270.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.West;
						break;
					case enCellDir.East:
						rotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.East;
						break;
				}
			break;
			
			case enCellType.corner_clockwise:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						rotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.East;
						break;
					case enCellDir.South:
						rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.West;
						break;
					case enCellDir.West:
						rotation = Quaternion.Euler (0.0f, 270.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.North;
						break;
					case enCellDir.East:
						rotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.South;
						break;
				}
			break;
			
			case enCellType.corner_anticlockwise:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						rotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.West;
						break;
					case enCellDir.South:
						rotation = Quaternion.Euler (0.0f, 270.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.East;
						break;
					case enCellDir.West:
						rotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.South;
						break;
					case enCellDir.East:
						rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.North;
						break;
				}
			break;
			
			case enCellType.corner_T:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						rotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.West;
						newCellInfo.EnterDirection = enCellDir.North;
						break;
					case enCellDir.South:
						rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.East;
						newCellInfo.EnterDirection = enCellDir.South;
						break;
					case enCellDir.West:
						rotation = Quaternion.Euler (0.0f, 270.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.South;
						newCellInfo.EnterDirection = enCellDir.West;
						break;
					case enCellDir.East:
						rotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
						newCellInfo.CellDirection = enCellDir.North;
						newCellInfo.EnterDirection = enCellDir.East;
						break;
				}
			break;
			
		}
		
		//Calculate position of cell
		switch (prevCellInfo.CellType) {
			
			case enCellType.straight:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						newCell_Position.z += (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.South:
						newCell_Position.z -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.West:
						newCell_Position.x -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.East:
						newCell_Position.x += (newCellInfo.length + prevCellInfo.length)/2;
						break;
				}
			break;
			
			case enCellType.corner_clockwise:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						newCell_Position.z += (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.South:
						newCell_Position.z -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.West:
						newCell_Position.x -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.East:
						newCell_Position.x += (newCellInfo.length + prevCellInfo.length)/2;
						break;
				}
			break;
			
			case enCellType.corner_anticlockwise:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						newCell_Position.z += (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.South:
						newCell_Position.z -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.West:
						newCell_Position.x -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.East:
						newCell_Position.x += (newCellInfo.length + prevCellInfo.length)/2;
						break;
				}
			break;
			
			case enCellType.corner_T:
				switch (prevCellInfo.CellDirection) {
					case enCellDir.North:
						newCell_Position.z += (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.South:
						newCell_Position.z -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.West:
						newCell_Position.x -= (newCellInfo.length + prevCellInfo.length)/2;
						break;
					case enCellDir.East:
						newCell_Position.x += (newCellInfo.length + prevCellInfo.length)/2;
						break;
				}
			break;
			
		}
		
		cellModel_new.transform.position = newCell_Position;
		cellModel_new.transform.rotation = rotation;
		
		return cellModel_new;
	}
	
	
	/// <summary>
	/// Rotates the cells.
	/// </summary>
	/// <param name='gameObject'>
	/// Game object.
	/// </param>
	public void rotateCells(GameObject gameObject){
		int num = 0;
		for(int i=0;i<m_cellObjects.Count;i++){
			if(m_cellObjects[i].Equals(gameObject)){
				num = i;
			}
		}//for
		
		CellInfo cellInfo_T = (CellInfo)gameObject.GetComponent("CellInfo");
		for(int i=num+1;i<m_cellObjects.Count;i++){
			m_cellObjects[i].transform.RotateAround (gameObject.transform.position, Vector3.up, 180);
			setDirectionForRotate(m_cellObjects[i]);
		}
	}
	
	
	/// <summary>
	/// Sets the direction for rotate.
	/// </summary>
	/// <param name='gameObject'>
	/// Game object.
	/// </param>
	private void setDirectionForRotate(GameObject gameObject){
		if(gameObject==null)
			return;
		CellInfo cellInfo = (CellInfo)gameObject.GetComponent("CellInfo");
		switch (cellInfo.CellDirection) {
			case enCellDir.North:
				cellInfo.CellDirection = enCellDir.South;
				break;
			case enCellDir.South:
				cellInfo.CellDirection = enCellDir.North;
				break;
			case enCellDir.West:
				cellInfo.CellDirection = enCellDir.East;
				break;
			case enCellDir.East:
				cellInfo.CellDirection = enCellDir.West;
				break;	
		}
	}
	
	
	/// <summary>
	/// Rules the analyse.
	/// </summary>
	/// <returns>
	/// The analyse.
	/// </returns>
	/// <param name='currentCellName'>
	/// Current cell name.
	/// </param>
	private enCellName ruleAnalyse(enCellName currentCellName){
		Dictionary<enCellName,int> tempDict = ruleMap[currentCellName];
		int loopCount = 0;
		int sum = 0;
		List<KeyValuePair<enCellName,int>> percents = new List<KeyValuePair<enCellName,int>>(tempDict.Count);

		//Calculate the sum
		foreach(KeyValuePair<enCellName,int> dict in tempDict){
			sum = sum + dict.Value;
		}
		//Calculating the probability of each item
		foreach(KeyValuePair<enCellName,int> dict in tempDict){
			int m = (dict.Value*100)/sum;
			if(loopCount>0)
				m = percents[loopCount-1].Value + m ;
			percents.Add(new KeyValuePair<enCellName,int>(dict.Key,m));
			loopCount++;
//			print(dict.Key+" : "+m);
		}
		//get one cellName by probability
		int temp = Random.Range(0,100);
		enCellName cellName = getCellByPrecent(temp, percents, 0);
//		print("cellName = "+cellName);
		return cellName;
	}
	
	
	/// <summary>
	/// Recursive function, get cellName by probability
	/// </summary>
	/// <returns>
	/// The cell by precent.
	/// </returns>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='percents'>
	/// Percents.
	/// </param>
	/// <param name='no'>
	/// No.
	/// </param>
	private enCellName getCellByPrecent(int target, List<KeyValuePair<enCellName,int>> percents, int no){
		if(no == percents.Count-1)
			return percents[no].Key;
		if(target < percents[no].Value)
			return percents[no].Key;
		else
			return getCellByPrecent(target, percents, no+1);
	}
	
	
	/// <summary>
	/// Gets the name of the game object by cell.
	/// </summary>
	/// <returns>
	/// The game object by cell name.
	/// </returns>
	/// <param name='name'>
	/// Name.
	/// </param>
	private GameObject getGameObjByCellName(enCellName name) {
		GameObject obj = m_tile_I1;
		switch (name) {
			case enCellName.I1:
				obj = m_tile_I1;
				break;
			case enCellName.I2:
				obj = m_tile_I2;
				break;
			case enCellName.I3:
				obj = m_tile_I3;
				break;
			case enCellName.I4:
				obj = m_tile_I4;
				break;
			case enCellName.LCw1:
				obj = m_tile_LCw1;
				break;
			case enCellName.LAcw1:
				obj = m_tile_LAcw1;
				break;
			case enCellName.T1:
				obj = m_tile_T1;
				break;
		}
		return obj;
	}
	
	
	/// <summary>
	/// Dynamic Creates the obstacle.
	/// </summary>
	/// <param name='cellModel'>
	/// Cell model.
	/// </param>
	private void createObstacle(GameObject cellModel){
		if(cellModel.transform.FindChild("obstacles")!=null){
			int count = cellModel.transform.FindChild("obstacles").childCount;
			int randomNum = Random.Range(0,count);
			for (int i = 0; i < count; i++){
				if(i == randomNum){
					cellModel.transform.FindChild("obstacles").GetChild(i).gameObject.SetActive(true);
				}
			}
			createCoin(cellModel, randomNum);
			createProp(cellModel, randomNum);
		}
	}
	
	
	/// <summary>
	/// Creates the coin.
	/// </summary>
	/// <param name='cellModel'>
	/// Cell model.
	/// </param>
	/// <param name='num'>
	/// Number.
	/// </param>
	private void createCoin(GameObject cellModel, int num){
		if(cellModel.transform.FindChild("coins")!=null){
			int count = cellModel.transform.FindChild("coins").childCount;
			for (int i = 0; i < count; i++){
				if(i == num){
					cellModel.transform.FindChild("coins").GetChild(i).gameObject.SetActive(true);
					string tag = cellModel.transform.FindChild("coins").GetChild(i).tag;
					if(tag!=null && tag=="tag_coins_curve"){
//						print ("--- tag_coins_curve");
						//sort
						adjustCoinPosition( cellModel.transform.FindChild("coins").GetChild(i).gameObject );
						cellModel.transform.FindChild("coins").GetChild(i).tag = null;
					}
				}
			}
		}
	}
	
	
	
	/// <summary>
	/// Creates the property.
	/// </summary>
	/// <param name='cellModel'>
	/// Cell model.
	/// </param>
	/// <param name='num'>
	/// Number.
	/// </param>
	private void createProp(GameObject cellModel, int num){
		if(cellModel.transform.FindChild("props")!=null){
			int count = cellModel.transform.FindChild("props").childCount;
			for (int i = 0; i < count; i++){
				if(i == num){
					int n = cellModel.transform.FindChild("props").GetChild(i).childCount;
					int randomNum = Random.Range(0,n);
					for(int  j= 0; j < n; j++){
						if(j == randomNum){
							Vector3 position_prop = cellModel.transform.FindChild("props").GetChild(i).GetChild(j).gameObject.transform.position;
							Instantiate (prefab_prop_magnet, position_prop, Quaternion.identity);
//							print ("--- show Prop !!!");
						}
					}
				}
			}
		}
	}
	
	
	
	/// <summary>
	/// Adjusts the coin position.
	/// </summary>
	/// <param name='coinGroup'>
	/// Coin group.
	/// </param>
	private void adjustCoinPosition(GameObject coinGroup){
		CellInfo cellInfo = (CellInfo)coinGroup.transform.parent.parent.GetComponent("CellInfo");
		int count = coinGroup.transform.childCount;
		
		for (int i = 0; i < count; i++){
			Transform coin_transfrom = coinGroup.transform.GetChild(i);
			Vector3 coin_position = coin_transfrom.position;
			//calculate new position of coin by cell direction
			if(cellInfo.CellDirection==enCellDir.North || cellInfo.CellDirection==enCellDir.South){
				float d = coin_position.z - coinGroup.transform.position.z;
				d = d*4;
				coin_position.z = coinGroup.transform.position.z + d;
			} else {
				float d = coin_position.x - coinGroup.transform.position.x;
				d = d*4;
				coin_position.x = coinGroup.transform.position.x + d;
			}
			//set position of coin
			coin_transfrom.position = coin_position;
		}
	}
	
	
		
}
