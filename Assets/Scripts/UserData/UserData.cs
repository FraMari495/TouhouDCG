using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    [SerializeField] public bool DoneTutorial = false;


    [SerializeField] private List<UserData_Card> remainedNumber = new List<UserData_Card>()
    {
        new UserData_Card(1,3),
        new UserData_Card(2,3),
        new UserData_Card(3,3),
        new UserData_Card(4,3),
        new UserData_Card(5,3),
        new UserData_Card(6,3),
        new UserData_Card(7,3),
        new UserData_Card(8,3),
        new UserData_Card(9,3),
        new UserData_Card(10,3),
        new UserData_Card(11,3),
        new UserData_Card(12,3),
        new UserData_Card(13,3),
        new UserData_Card(14,3),
        new UserData_Card(15,3),
        new UserData_Card(16,3),
        new UserData_Card(17,3),
    };

    [SerializeField]
    private List<UserData_Card> deck = new List<UserData_Card>()
    {
        new UserData_Card(1,3),
        new UserData_Card(2,3),
        new UserData_Card(3,3),
        new UserData_Card(4,3),
        new UserData_Card(5,3),
        new UserData_Card(6,3),
        new UserData_Card(7,3),
        new UserData_Card(8,3),
        new UserData_Card(9,3),
        new UserData_Card(10,3),
    };

    public List<UserData_Card> CardNumber { get => remainedNumber; }
    public List<UserData_Card> Deck { get => deck; set => deck = value; }

}


[System.Serializable]
public class UserData_Card
{
    public UserData_Card(int cardId,int number)
    {
        this.cardId = cardId;
        this.number = number;
    }

    [SerializeField] private int cardId;
    [SerializeField]private int number;

    public int CardId { get => cardId; }
    public int Number { get => number;  }

    public void Add()
    {
        number++;
    }

    public void Remove()
    {
        number--;
    }

}
