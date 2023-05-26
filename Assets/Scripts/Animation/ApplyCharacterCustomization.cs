using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ApplyCharacterCustomization : MonoBehaviour
{
    // Input Textures
    [Header("Base Textures")]
    [SerializeField]
    private Texture2D _maleFarmerBaseTexture = null;

    [SerializeField]
    private Texture2D _femaleFarmerBaseTexture = null;

    [SerializeField]
    private Texture2D _shirtsBaseTexture = null;

    private Texture2D _farmerBaseTexture;

    // Created Textures
    [Header("OutputBase Texture To Be Used For Animation")]
    [SerializeField]
    private Texture2D _farmerBaseCustomized = null;

    private Texture2D _farmerBaseShirtsUpdated;

    private Texture2D _selectedShirt;

    // Select Shirt Style
    [Header("Select Shirt Style")]
    [Range(0, 1)]
    [SerializeField]
    private int _inputShirtStyleNo = 0;

    // Select Sex
    [Header("Select Sex: 0=Male, 1=Female")]
    [Range(0, 1)]
    [SerializeField]
    private int _inputSex = 0;

    private Facing[,] _bodyFacingArray;
    private Vector2Int[,] _bodyShirtOffsetArray;

    // Dimensions
    private int _bodyRows = 21;

    private int _bodyColumns = 6;

    private int _farmerSpriteWidth = 16;

    private int _farmerSpriteHeight = 32;

    private int _shirtTextureWidth = 9;

    private int _shirtTextureHeight = 36;

    private int _shirtSpriteWidth = 9;

    private int _shirtSpriteHeight = 9;

    private int _shirtStylesInSpriteWidth = 16;

    private List<ColorSwap> _colorSwapList;

    // Target arm colors for color management
    private Color32 _armTargetColor1 = new Color32(77, 13, 13, 255);    // darkest

    private Color32 _armTargetColor2 = new Color32(138, 41, 41, 255);   // next darkest

    private Color32 _armTargetColor3 = new Color32(172, 50, 50, 255);   // lightest

    private void Awake()
    {
        _colorSwapList = new List<ColorSwap>();

        ProcessCustomization();
    }

    private void ProcessCustomization()
    {
        /*
        ProcessGender();

        ProcessShirt();

        ProcessArms();

        MergeCustomizations();
        */
    }

    private void ProcessGender()
    {
        _farmerBaseTexture = (_inputSex == 0) ? _maleFarmerBaseTexture : _femaleFarmerBaseTexture;

        _farmerBaseCustomized.SetPixels(_farmerBaseTexture.GetPixels());
        _farmerBaseCustomized.Apply();
    }

    private void ProcessShirt()
    {
        PopulateBodyFacingArray();

        // Initialize body shirt offset array
        _bodyShirtOffsetArray = new Vector2Int[_bodyColumns, _bodyRows];

        PopulateBodyShirtOffsetArray();

        AddShirtToTexture(_inputShirtStyleNo);

        ApplyShirtTextureToBase();
    }

    private void PopulateBodyFacingArray()
    {
        _bodyFacingArray = new Facing[_bodyColumns, _bodyRows];
        for (int col = 0; col < _bodyColumns; col++)
            for (int row = 0; row < _bodyRows; row++)
                _bodyFacingArray[col, row] = Facing.None;

        _bodyFacingArray[0, 10] = Facing.Back;
        _bodyFacingArray[1, 10] = Facing.Back;
        _bodyFacingArray[2, 10] = Facing.Back;
        _bodyFacingArray[3, 10] = Facing.Back;
        _bodyFacingArray[4, 10] = Facing.Back;
        _bodyFacingArray[5, 10] = Facing.Back;

        _bodyFacingArray[0, 11] = Facing.Front;
        _bodyFacingArray[1, 11] = Facing.Front;
        _bodyFacingArray[2, 11] = Facing.Front;
        _bodyFacingArray[3, 11] = Facing.Front;
        _bodyFacingArray[4, 11] = Facing.Back;
        _bodyFacingArray[5, 11] = Facing.Back;

        _bodyFacingArray[0, 12] = Facing.Back;
        _bodyFacingArray[1, 12] = Facing.Back;
        _bodyFacingArray[2, 12] = Facing.Right;
        _bodyFacingArray[3, 12] = Facing.Right;
        _bodyFacingArray[4, 12] = Facing.Right;
        _bodyFacingArray[5, 12] = Facing.Right;

        _bodyFacingArray[0, 13] = Facing.Front;
        _bodyFacingArray[1, 13] = Facing.Front;
        _bodyFacingArray[2, 13] = Facing.Front;
        _bodyFacingArray[3, 13] = Facing.Front;
        _bodyFacingArray[4, 13] = Facing.Back;
        _bodyFacingArray[5, 13] = Facing.Back;

        _bodyFacingArray[0, 14] = Facing.Back;
        _bodyFacingArray[1, 14] = Facing.Back;
        _bodyFacingArray[2, 14] = Facing.Right;
        _bodyFacingArray[3, 14] = Facing.Right;
        _bodyFacingArray[4, 14] = Facing.Right;
        _bodyFacingArray[5, 14] = Facing.Right;

        _bodyFacingArray[0, 15] = Facing.Front;
        _bodyFacingArray[1, 15] = Facing.Front;
        _bodyFacingArray[2, 15] = Facing.Front;
        _bodyFacingArray[3, 15] = Facing.Front;
        _bodyFacingArray[4, 15] = Facing.Back;
        _bodyFacingArray[5, 15] = Facing.Back;

        _bodyFacingArray[0, 16] = Facing.Back;
        _bodyFacingArray[1, 16] = Facing.Back;
        _bodyFacingArray[2, 16] = Facing.Right;
        _bodyFacingArray[3, 16] = Facing.Right;
        _bodyFacingArray[4, 16] = Facing.Right;
        _bodyFacingArray[5, 16] = Facing.Right;

        _bodyFacingArray[0, 17] = Facing.Front;
        _bodyFacingArray[1, 17] = Facing.Front;
        _bodyFacingArray[2, 17] = Facing.Front;
        _bodyFacingArray[3, 17] = Facing.Front;
        _bodyFacingArray[4, 17] = Facing.Back;
        _bodyFacingArray[5, 17] = Facing.Back;

        _bodyFacingArray[0, 18] = Facing.Back;
        _bodyFacingArray[1, 18] = Facing.Back;
        _bodyFacingArray[2, 18] = Facing.Right;
        _bodyFacingArray[3, 18] = Facing.Right;
        _bodyFacingArray[4, 18] = Facing.Right;
        _bodyFacingArray[5, 18] = Facing.Right;

        _bodyFacingArray[0, 19] = Facing.Right;
        _bodyFacingArray[1, 19] = Facing.Right;
        _bodyFacingArray[2, 19] = Facing.Right;
        _bodyFacingArray[3, 19] = Facing.Front;
        _bodyFacingArray[4, 19] = Facing.Front;
        _bodyFacingArray[5, 19] = Facing.Front;

        _bodyFacingArray[0, 20] = Facing.Front;
        _bodyFacingArray[1, 20] = Facing.Front;
        _bodyFacingArray[2, 20] = Facing.Front;
        _bodyFacingArray[3, 20] = Facing.Back;
        _bodyFacingArray[4, 20] = Facing.Back;
        _bodyFacingArray[5, 20] = Facing.Back;
    }

    private void PopulateBodyShirtOffsetArray()
    {
        _bodyShirtOffsetArray = new Vector2Int[_bodyColumns, _bodyRows];
        for (int col = 0; col < _bodyColumns; col++)
            for (int row = 0; row < _bodyRows; row++)
                _bodyShirtOffsetArray[col, row] = new Vector2Int(99, 99);

        _bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        _bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        _bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        _bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        _bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        _bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        _bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        _bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 11);
        _bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        _bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        _bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        _bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        _bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        _bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        _bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        _bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        _bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        _bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        _bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        _bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        _bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        _bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        _bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        _bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        _bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        _bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        _bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        _bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        _bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        _bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        _bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        _bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        _bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        _bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        _bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        _bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        _bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }

    private void AddShirtToTexture(int shirtStyleNo)
    {
        // create shirt texture
        _selectedShirt = new Texture2D(_shirtTextureHeight, _shirtTextureHeight);
        _selectedShirt.filterMode = FilterMode.Point;

        // Calculate coodinates for shirt pixels
        int x = (shirtStyleNo % _shirtStylesInSpriteWidth) * _shirtTextureWidth;
        int y = (shirtStyleNo / _shirtStylesInSpriteWidth) * _shirtTextureHeight;

        // Get shirts pixels
        var shirtPixels = _shirtsBaseTexture.GetPixels(x, y, _shirtTextureWidth, _shirtTextureHeight);

        // Apply selected shirt pixels to texture
        _selectedShirt.SetPixels(shirtPixels);
        _selectedShirt.Apply();
    }

    private void ApplyShirtTextureToBase()
    {
        // Create new shirt base texture
        _farmerBaseShirtsUpdated = new Texture2D(_farmerBaseTexture.width, _farmerBaseTexture.height);
        _farmerBaseShirtsUpdated.filterMode = FilterMode.Point;

        // Set shirt base texture to transparent
        SetTextureToTransparent(_farmerBaseShirtsUpdated);

        var frontShirtPixels = _selectedShirt.GetPixels(0, _shirtSpriteHeight * 3, _shirtSpriteWidth, _shirtSpriteHeight);
        var backShirtPixels = _selectedShirt.GetPixels(0, _shirtSpriteHeight * 0, _shirtSpriteWidth, _shirtSpriteHeight);
        var rightShirtPixels = _selectedShirt.GetPixels(0, _shirtSpriteHeight * 2, _shirtSpriteWidth, _shirtSpriteHeight);

        // Loop through base texture and apply shirt pixels

        for (int x = 0; x < _bodyColumns; x++)
        {
            for (int y = 0; y < _bodyRows; y++)
            {
                int pixelX = x * _farmerSpriteWidth;
                int pixelY = y * _farmerSpriteHeight;

                if (_bodyShirtOffsetArray[x, y].x == 99)    // do not populate with shirt
                    continue;

                pixelX += _bodyShirtOffsetArray[x, y].x;
                pixelY += _bodyShirtOffsetArray[x, y].y;

                // Switch on facing direction
                switch (_bodyFacingArray[x, y])
                {
                    case Facing.Front:
                        // Populate front shirt pixels
                        _farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, _shirtSpriteWidth, _shirtSpriteHeight, frontShirtPixels);
                        break;
                    case Facing.Back:
                        // Populate back shirt pixels
                        _farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, _shirtSpriteWidth, _shirtSpriteHeight, backShirtPixels);
                        break;
                    case Facing.Right:
                        // Populate right shirt pixels
                        _farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, _shirtSpriteWidth, _shirtSpriteHeight, rightShirtPixels);
                        break;
                    case Facing.None:
                    default:
                        break;
                }
            }
        }

        // Apply shirt texture pixels
        _farmerBaseShirtsUpdated.Apply();
    }

    private void SetTextureToTransparent(Texture2D texture2D)
    {
        // Fill texture with transparency
        var fill = new Color[texture2D.height * texture2D.width];
        Array.Fill(fill, Color.clear);
        texture2D.SetPixels(fill);
    }

    private void ProcessArms()
    {
        // Get arm pixels to recoor
        var farmerPixelsToRecolor = _farmerBaseTexture.GetPixels(0, 0, 288, _farmerBaseTexture.height);

        PopuateArmColorSwapList();

        // Change arm colors
        ChangePixelColors(farmerPixelsToRecolor, _colorSwapList);

        // Set recolored pixels
        _farmerBaseCustomized.SetPixels(0, 0, 288, _farmerBaseTexture.height, farmerPixelsToRecolor);

        _farmerBaseCustomized.Apply();
    }

    private void ChangePixelColors(Color[] baseArray, List<ColorSwap> colorSwapList)
    {
        for (int x = 0; x < baseArray.Length; x++)
        {
            // Loop through color swap list
            if (colorSwapList.Count > 0)
            {
                for (int y = 0; y < colorSwapList.Count; y++)
                {
                    if (baseArray[x] == colorSwapList[y].FromColor)
                    {
                        baseArray[x] = colorSwapList[y].ToColor;
                        break;
                    }
                }
            }
        }
    }

    private void PopuateArmColorSwapList()
    {
        _colorSwapList.Clear();

        _colorSwapList.Add(new ColorSwap(_armTargetColor1, _selectedShirt.GetPixel(0, 7)));
        _colorSwapList.Add(new ColorSwap(_armTargetColor2, _selectedShirt.GetPixel(0, 6)));
        _colorSwapList.Add(new ColorSwap(_armTargetColor3, _selectedShirt.GetPixel(0, 5)));
    }

    private void MergeCustomizations()
    {
        // Farmer Shirt Pixels
        var farmerShirtPixels = _farmerBaseShirtsUpdated.GetPixels(0, 0, _bodyColumns * _farmerSpriteWidth, _farmerBaseTexture.height);

        // Farmer Pants Pixels
        var farmerPantsPixelsSelection = _farmerBaseTexture.GetPixels(288, 0, 96, _farmerBaseTexture.height);

        // Farmer Body Pixels
        var farmerBodyPixels = _farmerBaseCustomized.GetPixels(0, 0, _bodyColumns * _farmerSpriteWidth, _farmerBaseTexture.height);

        MergeColorArray(farmerBodyPixels, farmerPantsPixelsSelection);
        MergeColorArray(farmerBodyPixels, farmerShirtPixels);

        // Paste merged pixels
        _farmerBaseCustomized.SetPixels(0, 0, _bodyColumns * _farmerSpriteWidth, _farmerBaseTexture.height, farmerBodyPixels);

        _farmerBaseCustomized.Apply();
    }

    private void MergeColorArray(Color[] baseArray, Color[] mergeArray)
    {
        for (int x = 0; x < baseArray.Length; x++)
        {
            if (mergeArray[x].a <= 0)
                continue;

            // Merge array has color
            if (mergeArray[x].a >= 1)
            {
                // Fully replace
                baseArray[x] = mergeArray[x];
            }
            else
            {
                // Interpolate colors
                float alpha = mergeArray[x].a;

                baseArray[x].r += (mergeArray[x].r - baseArray[x].r) * alpha;
                baseArray[x].g += (mergeArray[x].g - baseArray[x].g) * alpha;
                baseArray[x].b += (mergeArray[x].b - baseArray[x].b) * alpha;
                baseArray[x].a += alpha;
            }
        }

    }
}
