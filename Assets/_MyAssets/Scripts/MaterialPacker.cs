using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialPacker : MonoBehaviour {
	class RenderComponent
	{
		public MeshFilter _meshFilter = null;
		public Renderer _renderer = null;
	}

	Dictionary<Texture2D, List<RenderComponent>> _renderCompDict = new Dictionary<Texture2D, List<RenderComponent>>();
	Rect[] _packedRects = null;
	Texture2D[] _packTextures = null;
	Texture2D _atlasTexture = null;
	Material _material = null;

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform)
		{
			if (!child.gameObject.activeSelf) continue;
			var renderer = child.GetComponentInChildren<MeshRenderer>();
			if (renderer)
			{
				var meshFilter = child.GetComponentInChildren<MeshFilter>();
				if (meshFilter)
				{
					var tex = renderer.sharedMaterial.mainTexture;
					if (tex && tex is Texture2D)
					{
						if(_material == null)
						{
							_material = renderer.material;
						}
						var tex2D = tex as Texture2D;
						if (!_renderCompDict.ContainsKey(tex2D))
						{
							_renderCompDict.Add(tex2D, new List<RenderComponent>());
						}
						var renderComp = new RenderComponent();
						renderComp._meshFilter = meshFilter;
						renderComp._renderer = renderer;
						_renderCompDict[tex2D].Add(renderComp);
					}
				}
			}
		}

		{
			_packTextures = new Texture2D[_renderCompDict.Count];
			_renderCompDict.Keys.CopyTo(_packTextures, 0);
			const int kTexSize = 2048;
			_atlasTexture = new Texture2D(kTexSize, kTexSize);
			_packedRects = _atlasTexture.PackTextures(_packTextures, 2, kTexSize, true);//need texture sets read/writable
			if(_packedRects == null)
			{
				Debug.LogError("Packing failer");
			}
		}

		{
			for(int iTex=0; iTex<_packTextures.Length; iTex++)
			{
				var renderComps = _renderCompDict[_packTextures[iTex]];
				var packedRect = _packedRects[iTex];
				Debug.Log("packedRect: i:"+iTex+", rect:"+packedRect);

				for(int iMeshFilter=0; iMeshFilter<renderComps.Count; iMeshFilter++)
				{
					var mesh = renderComps[iMeshFilter]._meshFilter.mesh;
					var uvs = mesh.uv;
					for(int iUV=0; iUV<uvs.Length; iUV++)
					{
						//UVを正規化しないと多分ムリ…？1以上の値はマテリアルのタイリングで再現できる？
						//マイナスは無理そうだが…
						Debug.Log("iUV:" + iUV + ", x:" + uvs[iUV].x + ", y:" + uvs[iUV].y);
						if(uvs[iUV].x < 0)
						{
							uvs[iUV].x = uvs[iUV].x * packedRect.width + packedRect.x;
						}
						else
						{
							uvs[iUV].x = uvs[iUV].x * packedRect.width + packedRect.x;
						}
						uvs[iUV].y = 1f - (uvs[iUV].y * packedRect.height + packedRect.y);
					}
					//mesh.uv = uvs;
					_material.mainTexture = _atlasTexture;
					//renderComps[iMeshFilter]._renderer.material = _material;
				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (_atlasTexture)
		{
			GUI.DrawTexture(new Rect(0, 0, 256, 256), _atlasTexture);
		}
	}
}
