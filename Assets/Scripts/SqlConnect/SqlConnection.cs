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
    public static IEnumerator PostRating(LSystemWrapper[] LSystems, Action done)
    {
        PostLSystemHelper helper = new PostLSystemHelper();
        helper.PopulationId = PopulationId.Value;
        if (ParentId.HasValue)
        {
            helper.ParentId = ParentId.Value;
        }
        helper.Hash = hash;
        helper.LSystems = LSystems;
        var jsonString = JsonUtility.ToJson(helper);

        Debug.Log("Uploading: " + jsonString);
        using (UnityWebRequest post = UnityWebRequest.Put(addRatingURL, jsonString))
        {
            post.method = "POST";
            post.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
            //post.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonString));
            //post.uploadHandler.contentType = "application/json";
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
            }
        }

        done();
    }

    // Get population from MYSQL database
    public static IEnumerator GetPopulation(int numLSystems, Action<LSystemWrapper[]> done)
    {
        Request:
            UnityWebRequest request = new UnityWebRequest(getPopulationURL + "?NumLSystems=" + numLSystems + 
                (ParentId != null ? "&ParentId=" + ParentId?.ToString() : "") + 
                ((ParentId.HasValue && ParentId.Value == PopulationId) || (!ParentId.HasValue && PopulationId != null) ? "&PopulationId=" + PopulationId : ""));
            request.downloadHandler = new DownloadHandlerBuffer();
            Debug.Log("Retreiving Population: " + request.url);
            yield return request.SendWebRequest();

        LSystemWrapper[] retreivedLSystems = null;
        if (request.error != null)
        {
            Debug.LogError("There was an error retreiving the population: " + request.error);
        }
        else
        {
            try
            {
                string response = request.downloadHandler.text;
                Debug.Log(response);
                var JSONObj = JsonUtility.FromJson<PostLSystemHelper>(response);
                if (JSONObj == null)
                {
                    goto Request;
                }
                ParentId = PopulationId;
                PopulationId = JSONObj.PopulationId;
                hash = JSONObj.Hash;

                //if (numLSystems > 0 && (JSONObj.LSystems == null || JSONObj.LSystems.Length != numLSystems))
                //{
                //    goto Request;
                //}
                retreivedLSystems = JSONObj.LSystems;
            }
            catch
            {
                goto Request;
            }
        }

        done(retreivedLSystems);
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
    public bool IsStarred;
    public string Axiom;
    public string Rules;

    public string GetString()
    {
        return Axiom + '~' + Rules;
    }
}

[Serializable]
public class PostLSystemHelper
{
    public int PopulationId;
    public string Hash;
    public int ParentId = -1;
    public LSystemWrapper[] LSystems;
}