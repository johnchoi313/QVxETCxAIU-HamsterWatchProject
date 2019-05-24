using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HamsterTextures : MonoBehaviour {
    
    public HamsterSkin[] hamsterSkins;
    public Texture[] faceTextures;
    public Texture[] bodyTextures;
    
    public void setFaceTexture(int index, int tex) {
        if(0 <= index && index < hamsterSkins.Length) {
            if(hamsterSkins[index].faceMat != null && faceTextures != null) {
                if(hamsterSkins[index].faceMat.mainTexture != faceTextures[tex % faceTextures.Length]) {
                    hamsterSkins[index].faceMat.mainTexture = faceTextures[tex % faceTextures.Length];
                }
            }
        }
    }
    
    public void setBodyTexture(int index,int tex) {
        if(0 <= index && index < hamsterSkins.Length) {
            if(hamsterSkins[index].bodyMat != null && bodyTextures != null) {
                if(hamsterSkins[index].bodyMat.mainTexture != bodyTextures[tex % bodyTextures.Length]) {
                    hamsterSkins[index].bodyMat.mainTexture = bodyTextures[tex % bodyTextures.Length];
                }
            }
        }
    }

}

[System.Serializable]
public class HamsterSkin {
    public Material faceMat;
    public Material bodyMat;
}