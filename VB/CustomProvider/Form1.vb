﻿Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraMap
Imports System.IO
Imports System.Drawing
Imports System.Diagnostics

Namespace CustomProvider
    Partial Public Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            ' Create a map control, set its dock style and add it to the form.
            Dim map As New MapControl()
            map.Dock = DockStyle.Fill
            Me.Controls.Add(map)

            ' Create a layer to load image tiles from a local map data provider.
            Dim imageTilesLayer As New ImageLayer()
            map.Layers.Add(imageTilesLayer)
            imageTilesLayer.DataProvider = New LocalProvider()

        End Sub

        Public Class LocalProvider
            Inherits MapDataProviderBase


            Private ReadOnly projection_Renamed As New SphericalMercatorProjection()

            Public Sub New()
                TileSource = New LocalTileSource(Me)
            End Sub

            Public Overrides ReadOnly Property Projection() As ProjectionBase
                Get
                    Return projection_Renamed
                End Get
            End Property

            Protected Overrides ReadOnly Property BaseSizeInPixels() As Size
                Get
                    Return New Size(Convert.ToInt32(LocalTileSource.TileSize * 2), Convert.ToInt32(LocalTileSource.TileSize * 2))
                End Get
            End Property
        End Class

        Public Class LocalTileSource
            Inherits MapTileSourceBase

            Private Const _tileSize1 As Integer = 256
            Public Const maxZoomLevel As Integer = 2
            Private directoryPath As String

            Public Overloads Shared ReadOnly Property TileSize As Integer
                Get
                    Return _tileSize1
                End Get
            End Property

            Friend Shared Function CalculateTotalImageSize(ByVal zoomLevel As Double) As Double
                If zoomLevel < 1.0 Then
                    Return zoomLevel * TileSize * 2
                End If
                Return Math.Pow(2.0, zoomLevel) * TileSize
            End Function

            Public Sub New(ByVal cacheOptionsProvider As ICacheOptionsProvider)
                MyBase.New(CInt((CalculateTotalImageSize(maxZoomLevel))), CInt((CalculateTotalImageSize(maxZoomLevel))), TileSize, TileSize, cacheOptionsProvider)
                Dim dir As New DirectoryInfo(Directory.GetCurrentDirectory())
                directoryPath = dir.Parent.Parent.FullName
            End Sub

            Public Overrides Function GetTileByZoomLevel(ByVal zoomLevel As Integer, ByVal tilePositionX As Integer, ByVal tilePositionY As Integer) As Uri
                If zoomLevel <= maxZoomLevel Then
                    Dim u As New Uri(String.Format("file://" & directoryPath & "\openstreetmap.org\Hybrid_{0}_{1}_{2}.png", zoomLevel, tilePositionX, tilePositionY))
                    Return u
                End If
                Return Nothing
            End Function
        End Class
    End Class
End Namespace
