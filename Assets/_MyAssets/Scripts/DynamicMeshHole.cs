using UnityEngine;
using System.Collections;

/*
・自由に穴を開ける
・落とし穴のある地面が壊れる→ベース地面が存在しないので落下
	→ベース地面って全体に存在する必要ある？
	　→やっぱり無いと雰囲気出ないと思うけどなぁ
・少なくとも穴は予め開けておく。
・ベース地面が始めから全体に敷き詰めてあるのか、ベース地面を自身で置いていくのかの違い
・初めから全体を覆っている方が良いと考える
	→問題は穴の開け方か
		→1x1か4x4ぐらいが良いと考える。これは普通に穴を置く
			→設置した穴から光は自動で差し込む？
				→いや、これもユーザに設置させる。やっぱり集光装置的な好都合なやつで

・結局Meshに対して何をする？
	→見た目＝コリジョンなので、同じMeshをMeshColliderにセットできる？
		→ぶっちゃけベース地面は平地で、穴はグリッド単位で開くのでその辺コリジョンは嘘をつける（オブジェクト側の対応が必要）
	→厚みが必要なのでBoxである必要がある

・やり方
	→普通にBoxをおいて、ランタイムでCombineMeshして一つのMeshに結合
		→見えない部分のMeshが無駄になる。といっても頂点数は変わらないか。トライアングル数が増える

	→結局なるべく数が最小になるようにCubeをスケールして敷き詰めるのがクリエイトでの編集性と、パフォーマンスのバランスが良さそう？
		→とりあえず左奥から右へ穴にぶつかるまで行って、そこから下へ穴か端へぶつかるまで行って四角を作っていく
			→NG
		→１枚のQuadからスタート。この中に四角（穴）を置く
			置いたら、四角の上側、左、右、下側に新たにQuadを作る。上下の領域はその左右側も領域とする。つまり上下は横に広いQuadになる
			あとはこれを繰り返して領域を作っていく。
			領域をまたぐ形で四角を置かれた場合どうするか？
				→境界で四角を分割してそれぞれの領域に対して同じく処理を行う

*/
public class DynamicMeshHole : MonoBehaviour
{
	[SerializeField]
	MeshFilter _meshFilter = null;
	Mesh _mesh = null;
	bool _isHit = false;
	Vector3[] _hitVertices = new Vector3[3];

	// Use this for initialization
	void Start()
	{
		_mesh = _meshFilter.mesh;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Mesh mesh = _mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
				_hitVertices[0] = p0;
				_hitVertices[1] = p1;
				_hitVertices[2] = p2;
				_isHit = true;
			}
		}
		if (_isHit)
		{
			Debug.DrawLine(_hitVertices[0], _hitVertices[1]);
			Debug.DrawLine(_hitVertices[1], _hitVertices[2]);
			Debug.DrawLine(_hitVertices[2], _hitVertices[0]);
		}
	}

	//void OnRenderObject()
	//{
	//	Graphics.DrawProcedural(MeshTopology.)
	//}
}
