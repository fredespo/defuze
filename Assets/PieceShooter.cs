﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceShooter : MonoBehaviour
{
    public GameObject pieceParent;
    public bool shootingEnabled = false;
    public GameObject[] pieces;
    public int pieceIndex = 0;
    public float speed = 1.0f;
    public AngleChangeMode angleChangeMode;
    private GameObject spawnedPiece;
    private bool spawnedPieceReadyToShoot = false;
    private float[] angles;
    private int angleIdx;

    public void Init()
    {
        pieceIndex = 0;
        angleIdx = 0;
        SetAngle(angles[angleIdx]);

        if (angleChangeMode == AngleChangeMode.CONTINUOUSLY)
        {
            StartCoroutine("ChangeAngleContinuously");
        }
        else
        {
            StopCoroutine("ChangeAngleContinuously");
        }

        SpawnPiece();
    }

    void Update()
    {
        if (spawnedPiece == null && CanSpawnPiece())
        {
            OnShoot();
        }
        else if(spawnedPieceReadyToShoot && spawnedPiece != null)
        {
            spawnedPiece.SetActive(shootingEnabled);
        }

        if (shootingEnabled)
        {
            if (spawnedPieceReadyToShoot && spawnedPiece != null)
            {
                Vector3 pieceRotation = spawnedPiece.transform.eulerAngles;
                spawnedPiece.transform.eulerAngles = new Vector3(pieceRotation.x, pieceRotation.y, transform.eulerAngles.z);
            }
        }
    }

    private void OnShoot()
    {
        if(angleChangeMode == AngleChangeMode.ON_SHOOT)
        {
            ChangeAngle();
        }
        SpawnPiece();
    }

    IEnumerator ChangeAngleContinuously()
    {
        while(angleChangeMode == AngleChangeMode.CONTINUOUSLY)
        {
            yield return new WaitForSeconds(1.0f);
            ChangeAngle();
        }
    }

    public void Shoot()
    {
        if (shootingEnabled && spawnedPiece != null && spawnedPieceReadyToShoot)
        {
            spawnedPiece.GetComponent<Rigidbody2D>().velocity = transform.up * speed;
            spawnedPieceReadyToShoot = false;
        }
    }

    private void SetAngle(float angle)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
    }

    private void ChangeAngle()
    {
        if(++angleIdx >= angles.Length)
        {
            angleIdx = 0;
        }
        SetAngle(angles[angleIdx]);
    }

    void SpawnPiece()
    {
        if (!CanSpawnPiece())
        {
            return;
        }

        if(GameObject.FindGameObjectsWithTag(pieces[pieceIndex].transform.tag).Length == 0)
        {
            ++pieceIndex;
        }

        if(pieceIndex < pieces.Length)
        {
            spawnedPiece = Instantiate(pieces[pieceIndex], gameObject.transform.position, gameObject.transform.rotation);
            spawnedPiece.transform.SetParent(pieceParent.transform);
            spawnedPiece.transform.position = gameObject.transform.position;
            spawnedPiece.transform.localScale = new Vector3(81, 81, 1);
        }

        spawnedPieceReadyToShoot = true;
    }

    public void SetShootingEnabled(bool shootingEnabled)
    {
        this.shootingEnabled = shootingEnabled;
    }

    public void SetAngleChangeMode(AngleChangeMode mode)
    {
        angleChangeMode = mode;
    }

    public void SetAngles(float[] angles)
    {
        this.angles = angles;
    }

    public enum AngleChangeMode
    {
        ON_SHOOT, CONTINUOUSLY
    }

    public bool IsSpawnedPieceReadyToShoot()
    {
        return this.spawnedPieceReadyToShoot;
    }

    public void Inactivate()
    {
        gameObject.SetActive(false);
    }

    private bool CanSpawnPiece()
    {
        return pieceIndex >= 0 && pieceIndex < pieces.Length;
    }
}
