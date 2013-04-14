#pragma strict

var m_camera : Camera;
var player_name : String;

function ChangeName(name : String)
{
	var text_component = transform.GetComponent("TextMesh") as TextMesh;
	text_component.text = name;
}

function Update () {
	if(m_camera != null) {
		transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward,
			m_camera.transform.rotation * Vector3.up);
	}
}