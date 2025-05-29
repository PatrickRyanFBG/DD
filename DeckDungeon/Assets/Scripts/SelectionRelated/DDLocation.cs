using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DDLocation : DDSelection
{
    [SerializeField] private Vector2Int coord;
    public Vector2Int Coord => coord;

    private DDEnemyOnBoard currentEnemy;

    [SerializeField] private Transform effectsParent;

    [SerializeField] private DDLocationEffectVisuals visualsPrefab;

    private List<DDLocationEffectBase> effects = new List<DDLocationEffectBase>();
    private List<DDLocationEffectVisuals> visuals = new List<DDLocationEffectVisuals>();

    [SerializeField] private TMPro.TextMeshProUGUI info;

    private Vector2Int meleeRangeBonus;

    [Header("Testing")] [SerializeField] private Renderer hoveredRender;

    [SerializeField] private Renderer highlightRenderer;

    [SerializeField] private Vector2 minMaxX;

    [SerializeField] private RawImage spawnedImage;
    
    [ContextMenu("Fix Coord")]
    private void FixCoord()
    {
        /*
        if (transform.position.z == -3)
        {
            coord.y = 0;
        }
        else if (transform.position.z == -1.25f)
        {
            coord.y = 1;
        }
        else if (transform.position.z == 0.5f)
        {
            coord.y = 2;
        }
        else if (transform.position.z == 2.25f)
        {
            coord.y = 3;
        }
        else if (transform.position.z == 4f)
        {
            coord.y = 4;
        }

        if (transform.position.x == -5)
        {
            coord.x = 0;
        }
        else if (transform.position.x == -3)
        {
            coord.x = 1;
        }
        else if (transform.position.x == -1)
        {
            coord.x = 2;
        }
        else if (transform.position.x == 1)
        {
            coord.x = 3;
        }
        else if (transform.position.x == 3)
        {
            coord.x = 4;
        }
        else if (transform.position.x == 5)
        {
            coord.x = 5;
        }
        */
        gameObject.name = "Location " + coord.x + " " + coord.y;
    }

    public void FixCoordX(int x)
    {
        coord.x = x;
        gameObject.name = "Location " + coord.x + " " + coord.y;
    }

    public void FixCoord(int y)
    {
        coord.y = y;
        gameObject.name = "Location " + coord.x + " " + coord.y;
    }

    public void SetMeleeRangeBonus(Vector2Int bonus)
    {
        meleeRangeBonus = bonus;

        if (bonus == Vector2Int.zero)
        {
            info.text = "";
        }
        else
        {
            info.text = "Ranged: " + (bonus.y > 0 ? "+" : "") + bonus.y + "\r\nMelee: " + (bonus.x > 0 ? "+" : "") + bonus.x;
        }
    }

    public override void Hovered(bool fromAnotherSelection = false)
    {
        base.Hovered(fromAnotherSelection);

        hoveredRender.enabled = true;
    }

    public override void Unhovered(bool fromAnotherSelection = false)
    {
        hoveredRender.enabled = false;
    }

    public void Highlight()
    {
        highlightRenderer.enabled = true;
    }

    public void Unhighlight()
    {
        highlightRenderer.enabled = false;
    }

    public bool HasEnemy()
    {
        return currentEnemy;
    }

    public DDEnemyOnBoard GetEnemy()
    {
        return currentEnemy;
    }

    public void SnapEnemyToHere(DDEnemyOnBoard enemy)
    {
        currentEnemy = enemy;
        if (enemy)
        {
            enemy.SnapLocation(this);
        }
    }

    public IEnumerator SetEnemy(DDEnemyOnBoard enemy)
    {
        if (currentEnemy)
        {
            yield return currentEnemy.SetLocation(null);
        }

        currentEnemy = enemy;

        if (currentEnemy)
        {
            yield return currentEnemy.SetLocation(this);
        }
    }

    public void AddEffect(DDLocationEffectBase effect, Texture image)
    {
        effects.Add(effect);
        DDLocationEffectVisuals vis = Instantiate(visualsPrefab, effectsParent);
        vis.Init(image, effect.TurnsLeft);
        visuals.Add(vis);
    }

    public void DoEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].DoEffect(this))
            {
                effects.RemoveAt(i);
                Destroy(visuals[i].gameObject);
                visuals.RemoveAt(i);
                --i;
            }
            else
            {
                visuals[i].UpdateTurns(effects[i].TurnsLeft);
            }
        }
    }

    public void ClearEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects.RemoveAt(i);
            Destroy(visuals[i].gameObject);
            visuals.RemoveAt(i);
        }

        effects.Clear();
        visuals.Clear();
    }

    public override void FillEnemyList(ref List<DDEnemyOnBoard> enemies)
    {
        if (currentEnemy)
        {
            enemies.Add(currentEnemy);
        }
    }

    public void ShowSpawnedEntity(Texture entity)
    {
        spawnedImage.texture = entity;
        spawnedImage.enabled = true;
    }

    public void HideSpawnedEntity()
    {
        spawnedImage.enabled = false;
    }
}

public class DDLocationEffectBase
{
    private int turnsLeft;
    public int TurnsLeft => turnsLeft;

    public DDLocationEffectBase(int forTurns)
    {
        turnsLeft = forTurns;
    }

    public virtual bool DoEffect(DDLocation loc)
    {
        --turnsLeft;

        return turnsLeft <= 0;
    }
}