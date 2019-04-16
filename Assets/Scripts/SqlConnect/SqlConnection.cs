using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SqlConnection 
{
    private string secretKey = "aiingames"; // Edit this value and make sure it's the same as the one stored on the server
    public string addRatingURL = "https://hispid-compounds.000webhostapp.com/php/addrating.php?"; //be sure to add a ? to your url
    
    // remember to use StartCoroutine when calling this function!
    public IEnumerator PostRating(int LSystemId, int rating)
    {
        string hash = Md5Sum(LSystemId.ToString() + rating.ToString() + secretKey);

        string post_url = addRatingURL + "LSystemId=" + LSystemId + "&rating=" + rating + "&hash=" + hash;

        // Post the URL to the site and create a download object to get the result.
        UnityWebRequest rating_post = new UnityWebRequest(post_url);
        yield return rating_post; // Wait until the download is done

        if (rating_post.error != null)
        {
            Debug.Log("There was an error posting the rating: " + rating_post.error);
        }
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}
