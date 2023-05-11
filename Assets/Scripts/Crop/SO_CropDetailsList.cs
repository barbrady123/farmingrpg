using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop/Crop Details List")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField]
    public List<CropDetails> CropDetails;

    public CropDetails GetCropDetails(int seedItemCode) => this.CropDetails.SingleOrDefault(x => x.SeedItemCode == seedItemCode);

}
