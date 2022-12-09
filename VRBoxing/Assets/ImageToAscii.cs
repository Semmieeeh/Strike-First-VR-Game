using UnityEngine;
using UnityEngine.UI;

public class ImageToAscii : MonoBehaviour
{
    // Reference to the input image
    public Texture2D texture;

    // Reference to the text object to display the output
    public Text text;

    // List of characters to use for the output
    private string chars = "`^\",:;Il!i~+_-?][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$";

    void Start()
    {
        // Get the pixel data from the input image
        Color[] pixels = texture.GetPixels();

        // Create a string for the output
        string output = "";

        // Map each pixel to a character in the list
        foreach (Color pixel in pixels)
        {
            // Calculate the average value of the RGB components
            float value = (pixel.r + pixel.g + pixel.b) / 3;

            // Map the value to a character in the list
            int index = Mathf.RoundToInt(value * (chars.Length - 1));
            char c = chars[index];

            // Add the character to the output string
            output += c;
        }

        // Set the text of the output text object to the generated string
        text.text = output;
    }
}
