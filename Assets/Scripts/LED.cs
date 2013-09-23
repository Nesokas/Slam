using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LED : MonoBehaviour {
	
	public Material led_on;
	public Material led_off;
	public int current_number = 0;
	
	private Dictionary<int, List<int[]> > numbers;
	

	// Use this for initialization
	void Start () {
		
		numbers = new Dictionary<int, List<int[]> >();
		for(int i = 0; i < 10; i++) {
			numbers.Add(i, new List<int[]>());
		}
		
		/*************** 0 **************/
		numbers[0].Add(new int[3]{1,1,1});
		numbers[0].Add(new int[3]{1,0,1});
		numbers[0].Add(new int[3]{1,0,1});
		numbers[0].Add(new int[3]{1,0,1});
		numbers[0].Add(new int[3]{1,1,1});
		/********************************/
		
		/*************** 1 **************/
		numbers[1].Add(new int[3]{0,0,1});
		numbers[1].Add(new int[3]{0,0,1});
		numbers[1].Add(new int[3]{0,0,1});
		numbers[1].Add(new int[3]{0,0,1});
		numbers[1].Add(new int[3]{0,0,1});
		/********************************/
		
		/*************** 2 **************/
		numbers[2].Add(new int[3]{1,1,1});
		numbers[2].Add(new int[3]{0,0,1});
		numbers[2].Add(new int[3]{1,1,1});
		numbers[2].Add(new int[3]{1,0,0});
		numbers[2].Add(new int[3]{1,1,1});
		/********************************/
		
		/*************** 3 **************/
		numbers[3].Add(new int[3]{1,1,1});
		numbers[3].Add(new int[3]{0,0,1});
		numbers[3].Add(new int[3]{1,1,1});
		numbers[3].Add(new int[3]{0,0,1});
		numbers[3].Add(new int[3]{1,1,1});
		/********************************/
		
		/*************** 4 **************/
		numbers[4].Add(new int[3]{1,0,1});
		numbers[4].Add(new int[3]{1,0,1});
		numbers[4].Add(new int[3]{1,1,1});
		numbers[4].Add(new int[3]{0,0,1});
		numbers[4].Add(new int[3]{0,0,1});
		/********************************/
		
		/*************** 5 **************/
		numbers[5].Add(new int[3]{1,1,1});
		numbers[5].Add(new int[3]{1,0,0});
		numbers[5].Add(new int[3]{1,1,1});
		numbers[5].Add(new int[3]{0,0,1});
		numbers[5].Add(new int[3]{1,1,1});
		/********************************/
		
		/*************** 6 **************/
		numbers[6].Add(new int[3]{1,1,1});
		numbers[6].Add(new int[3]{1,0,0});
		numbers[6].Add(new int[3]{1,1,1});
		numbers[6].Add(new int[3]{1,0,1});
		numbers[6].Add(new int[3]{1,1,1});
		/********************************/
		
		/*************** 7 **************/
		numbers[7].Add(new int[3]{1,1,1});
		numbers[7].Add(new int[3]{0,0,1});
		numbers[7].Add(new int[3]{0,0,1});
		numbers[7].Add(new int[3]{0,0,1});
		numbers[7].Add(new int[3]{0,0,1});
		/********************************/
		
		/*************** 8 **************/
		numbers[8].Add(new int[3]{1,1,1});
		numbers[8].Add(new int[3]{1,0,1});
		numbers[8].Add(new int[3]{1,1,1});
		numbers[8].Add(new int[3]{1,0,1});
		numbers[8].Add(new int[3]{1,1,1});
		/********************************/
		
		/*************** 9 **************/
		numbers[9].Add(new int[3]{1,1,1});
		numbers[9].Add(new int[3]{1,0,1});
		numbers[9].Add(new int[3]{1,1,1});
		numbers[9].Add(new int[3]{0,0,1});
		numbers[9].Add(new int[3]{1,1,1});
		/********************************/
		
			
	}
	
	// Update is called once per frame
	void Update () {
		
		List<int[]> matrix = numbers[current_number];
		
		for(int i = 0; i < matrix.Count; i++) {
			for(int j = 0; j < matrix[i].Length; j++) {
				Material material_to_use;
				if(matrix[i][j] == 0)
					material_to_use = led_off;
				else
					material_to_use = led_on;
				
				string name = i + "_" + j;
				
				GameObject led = transform.Find(name).gameObject;
				led.transform.renderer.material = material_to_use;
			}
		}
	}
	
	public void SetCurrentNumber(int number)
	{
		current_number = number;
	}
	
	
}
