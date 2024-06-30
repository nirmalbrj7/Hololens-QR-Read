using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.QR;
using System;
using System.Collections.Generic;

public class QRCodeScanner : MonoBehaviour
{
    public static QRCodeScanner Instance;

    private QRCodeWatcher qrTracker;
    private bool isInitialized = false;
    private Dictionary<Guid, QRInfo> qrCodesList = new Dictionary<Guid, QRInfo>();

    [SerializeField]
    private QRPopup qrPopup;

    private void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        try
        {
            await QRCodeWatcher.RequestAccessAsync();
            qrTracker = new QRCodeWatcher();
            qrTracker.Added += QrTracker_Added;
            qrTracker.Updated += QrTracker_Updated;
            qrTracker.Removed += QrTracker_Removed;
            qrTracker.Start();
            isInitialized = true;
            Debug.Log("QR Code tracking started.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize QR tracking: {ex.Message}");
        }
    }

    private void QrTracker_Added(object sender, QRCodeAddedEventArgs e)
    {
        lock (qrCodesList)
        {
            qrCodesList[e.Code.Id] = new QRInfo() { Data = e.Code.Data, LastDetectedTime = DateTimeOffset.Now };
        }
        ShowQRDetails(e.Code.Data);
    }

    private void QrTracker_Updated(object sender, QRCodeUpdatedEventArgs e)
    {
        lock (qrCodesList)
        {
            qrCodesList[e.Code.Id].LastDetectedTime = DateTimeOffset.Now;
        }
    }

    private void QrTracker_Removed(object sender, QRCodeRemovedEventArgs e)
    {
        lock (qrCodesList)
        {
            qrCodesList.Remove(e.Code.Id);
        }
    }

    void ShowQRDetails(string qrContent)
    {
        qrPopup.ShowPopup(qrContent);
    }

    private void OnDestroy()
    {
        if (isInitialized)
        {
            qrTracker.Added -= QrTracker_Added;
            qrTracker.Updated -= QrTracker_Updated;
            qrTracker.Removed -= QrTracker_Removed;
            qrTracker.Stop();
        }
    }

    private class QRInfo
    {
        public string Data { get; set; }
        public DateTimeOffset LastDetectedTime { get; set; }
    }
}