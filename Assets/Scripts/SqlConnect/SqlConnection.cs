using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SqlConnection
{
    static string addRatingURL = "https://hispid-compounds.000webhostapp.com/php/AddRating.php"; //be sure to add a ? to your url
    static string getPopulationURL = "https://hispid-compounds.000webhostapp.com/php/GetPopulation.php";

    public static int? PopulationId { get; set; }
    public static int? ParentId { get; set; }
    public static string hash { get; set; }

    // remember to use StartCoroutine when calling this function!
    public static IEnumerator PostRating(LSystemWrapper[] LSystems)
    {
        //string hash = Md5Sum(LSystemId.ToString() + rating.ToString() + secretKey);

        //string post_url = addRatingURL + "LSystemId=" + LSystemId + "&rating=" + rating + "&hash=" + hash;
        //WWWForm form = new WWWForm();
        //form.AddField("hash", hash);
        //form.AddField("PopulationId", PopulationId?.ToString());
        //form.AddField("ParentId", ParentId?.ToString());
        //foreach (var rating in ratings)
        //{
        //    form.AddField("lsystem" + rating.Key.ToString(), rating.Value.ToString());
        //}

        //Debug.Log(post_url);
        // Post the URL to the site and create a download object to get the result.
        //UnityWebRequest post = new UnityWebRequest(addRatingURL);
        //post.SetRequestHeader("Content-type", "application/json");

        PostLSystemHelper helper = new PostLSystemHelper();
        helper.PopulationId = PopulationId.Value;
        helper.ParentId = ParentId;
        helper.Hash = hash;
        helper.LSystems = LSystems;
        var jsonString = JsonUtility.ToJson(helper);

        //WWWForm form = new WWWForm();
        //form.AddField("data", jsonString);
        //post.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));

        using (UnityWebRequest post = UnityWebRequest.Post(addRatingURL, jsonString))
        {
            post.SetRequestHeader("Content-Type", "application/json");
            post.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
            post.uploadHandler.contentType = "application/json";
            //post.downloadHandler = new DownloadHandlerBuffer();
            //post.chunkedTransfer = false;
            yield return post.SendWebRequest();

            if (post.isNetworkError || post.isHttpError)
            {
                Debug.LogError(post.error);
            }
            else
            {
                Debug.Log(post.downloadHandler.text);
                ParentId = PopulationId;
            }
        }
    }

    // Get population from MYSQL database
    public static IEnumerator GetPopulation()
    {
        UnityWebRequest request = new UnityWebRequest(getPopulationURL + (ParentId != null ? "?ParentId=" + ParentId?.ToString() : ""));
        request.downloadHandler = new DownloadHandlerBuffer();
        Debug.Log("Retreiving Population");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            Debug.LogError("There was an error retreiving the population: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            //string[] variables = response.Split('|');
            var JSONObj = JsonUtility.FromJson<Population>(response);
            PopulationId = JSONObj.PopulationId;
            hash = JSONObj.Hash;
            Debug.Log("PopulationId: " + PopulationId);
        }
    }
}

[Serializable]
public class Population
{
    public int PopulationId;
    public string Hash;
}

[Serializable]
public class LSystemWrapper
{
    public int PopulationId;
    public int LSystemId;
    public int Ranking;
    public string Axiom;
    public string Rules;
}

[Serializable]
public class PostLSystemHelper
{
    public int PopulationId;
    public string Hash;
    public int? ParentId;
    public LSystemWrapper[] LSystems;
}