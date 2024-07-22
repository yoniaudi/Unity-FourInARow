using System.Linq;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardGameModel : MonoBehaviour
{
    [SerializeField] private GameObject WhiteGamePiece = null;
    [SerializeField] private GameObject m_ColumnCollider = null;
    private Color[] m_GamePieceColors = new Color[] { Color.green, Color.red, Color.red };
    private GameObject[,] m_Board = null;
    private BoxCollider2D[] m_ColumnColliders = null;

    public void Initialize(int i_RowLength, int i_ColumnLength)
    {
        GameObject gamePiecesGroup = new GameObject("Game Pieces");
        GameObject gameCollidersGroup = new GameObject("Column Colliders");
        Bounds boardBounds = this.GetComponent<Renderer>().bounds;
        float pieceWidth = boardBounds.size.x / i_ColumnLength;
        float pieceHeight = boardBounds.size.y / i_RowLength;
        float pieceSize = Mathf.Min(pieceWidth, pieceHeight);
        float totalWidth = pieceSize * i_ColumnLength;
        float totalHeight = pieceSize * i_RowLength;
        Vector3 startPosition = new Vector3(
            boardBounds.center.x - (totalWidth / 2),
            boardBounds.center.y + (totalHeight / 2) - pieceSize,
            boardBounds.center.z
        );

        m_Board = new GameObject[i_RowLength, i_ColumnLength];
        m_ColumnColliders = new BoxCollider2D[i_ColumnLength];
        gamePiecesGroup.transform.SetParent(this.transform);
        gameCollidersGroup.transform.SetParent(this.transform);

        for (int i = 0; i < i_RowLength; i++)
        {
            for (int j = 0; j < i_ColumnLength; j++)
            {
                float xPosition = startPosition.x + (j * pieceSize) + (pieceSize / 2);
                float yPosition = startPosition.y - (i * pieceSize) + (pieceSize / 2);
                float pieceSizeWithOffset = pieceSize - pieceSize * 0.1f;
                Vector3 position = new Vector3(xPosition, yPosition, boardBounds.center.z);
                GameObject gamePiece = Instantiate(WhiteGamePiece, position, Quaternion.identity);

                gamePiece.gameObject.name = $"GamePiece {i} x {j}";
                gamePiece.gameObject.transform.localScale = new Vector3(pieceSizeWithOffset, pieceSizeWithOffset, pieceSizeWithOffset);
                gamePiece.transform.SetParent(gamePiecesGroup.transform);
                gamePiece.SetActive(false);
                m_Board[i, j] = gamePiece;

                if (i == 0)
                {
                    GameObject boardColumn = Instantiate(m_ColumnCollider);

                    boardColumn.gameObject.name = $"ColumnCollider {j + 1}";
                    boardColumn.transform.localPosition = position;
                    boardColumn.transform.SetParent(gameCollidersGroup.transform);
                    m_ColumnColliders[j] = boardColumn.GetComponent<BoxCollider2D>();
                    m_ColumnColliders[j].size = new Vector2(pieceSize, boardBounds.size.y);
                    m_ColumnColliders[j].transform.localPosition = new Vector3(m_ColumnColliders[j].transform.localPosition.x,
                        boardBounds.center.y, m_ColumnColliders[j].transform.localPosition.z);
                }
            }
        }
    }

    public void Show()
    {
        foreach (GameObject gamePiece in m_Board)
        {
            gamePiece.SetActive(true);
        }

        foreach (BoxCollider2D collider in m_ColumnColliders)
        {
            collider.enabled = true;
        }
    }

    public void ResetBoard()
    {
        foreach (GameObject gamePiece in m_Board)
        {
            gamePiece.GetComponent<SpriteRenderer>().color = Color.white;
        }

        foreach (BoxCollider2D collider in m_ColumnColliders)
        {
            collider.enabled = true;
        }
    }

    public void SetColumnCollidersActive(bool i_IsActive)
    {
        foreach (BoxCollider2D collider in m_ColumnColliders)
        {
            collider.enabled = i_IsActive;
        }
    }

    public bool IsColumnFull(int i_UserNextMoveIndx)
    {
        return m_Board[0, i_UserNextMoveIndx].GetComponent<SpriteRenderer>().color != Color.white;
    }

    public bool InsertUserNextMoveToBoard(Color i_GamePieceColor, int i_UserNextMoveIndx, ref int o_PlayerRowInsertPosition)
    {
        bool isNextMoveInserted = false;

        if (i_UserNextMoveIndx >= 0 && i_UserNextMoveIndx < m_Board.GetLength(1))
        {
            for (int i = m_Board.GetLength(0) - 1; i >= 0; i--)
            {
                SpriteRenderer gamePieceSpriteRenderer = m_Board[i, i_UserNextMoveIndx].GetComponent<SpriteRenderer>();

                if (gamePieceSpriteRenderer.color == Color.white)
                {
                    gamePieceSpriteRenderer.color = m_GamePieceColors[(int)GameManager.m_Instance.m_TurnState];
                    o_PlayerRowInsertPosition = i;
                    isNextMoveInserted = true;
                    break;
                }
            }
        }

        return isNextMoveInserted;
    }

    public int GetRowLength()
    {
        return m_Board.GetLength(0);
    }

    public int GetColumnLength()
    {
        return m_Board.GetLength(1);
    }

    public bool IsTie()
    {
        bool isTopRowFull = true;

        for (int i = 0; i < m_Board.GetLength(0); i++)
        {
            if (m_Board[0, i].GetComponent<SpriteRenderer>().color == Color.white)
            {
                isTopRowFull = false;
                break;
            }
        }

        return isTopRowFull == true;
    }

    public bool CheckIfWinner(int i_LastRowMoveInput,  int i_LastColumnMoveInput, Color i_GamePieceColor)
    {
        bool isRowWin = checkForRowWin(i_LastRowMoveInput, i_LastColumnMoveInput, i_GamePieceColor);
        bool isColumnWin = checkForColumnWin(i_LastRowMoveInput, i_LastColumnMoveInput, i_GamePieceColor);
        bool isLeftDiagonalWin = checkForLeftDiagonalWin(i_LastRowMoveInput, i_LastColumnMoveInput, i_GamePieceColor);
        bool isRightDiagonalWin = checkForRightDiagonalWin(i_LastRowMoveInput, i_LastColumnMoveInput, i_GamePieceColor);

        return isRowWin || isColumnWin || isLeftDiagonalWin || isRightDiagonalWin;
    }

    public GameObject[,] GetBoardGame()
    {
        return m_Board ?? null;
    }

    public void Reset()
    {
        foreach (GameObject gamePiece in m_Board)
        {
            Destroy(gamePiece);
        }

        foreach (BoxCollider2D columnCollider in m_ColumnColliders)
        {
            Destroy(columnCollider);
        }

        m_Board = null;
        m_ColumnColliders = null;
    }

    private bool checkForRowWin(int i_LastRowMoveInput, int i_LastColumnMoveInput, Color i_GamePieceColor)
    {
        bool isFourInARow = false;
        int neighborsCounter = 1;

        for (int i = i_LastColumnMoveInput + 1; i <= i_LastColumnMoveInput + 3; i++)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i >= m_Board.GetLength(1))
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i_LastRowMoveInput, i].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        for (int i = i_LastColumnMoveInput - 1; i >= i_LastColumnMoveInput - 3; i--)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i < 0)
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i_LastRowMoveInput, i].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        return isFourInARow;
    }

    private bool checkForColumnWin(int i_LastRowMoveInput, int i_LastColumnMoveInput, Color i_GamePieceColor)
    {
        bool isFourInARow = false;
        int neighborsCounter = 1;

        for (int i = i_LastRowMoveInput + 1; i <= i_LastRowMoveInput + 3; i++)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i >= m_Board.GetLength(0))
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i, i_LastColumnMoveInput].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        for (int i = i_LastRowMoveInput - 1; i >= i_LastRowMoveInput - 3; i--)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i < 0)
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i, i_LastColumnMoveInput].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        return isFourInARow;
    }

    private bool checkForRightDiagonalWin(int i_LastRowMoveInput, int i_LastColumnMoveInput, Color i_GamePieceColor)
    {
        bool isFourInARow = false;
        int neighborsCounter = 1;
        int i = 0;
        int j = 0;

        for (i = i_LastRowMoveInput - 1, j = i_LastColumnMoveInput + 1; i >= i_LastRowMoveInput - 3 && j <= i_LastColumnMoveInput + 3; i--, j++)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i < 0 || j >= m_Board.GetLength(1))
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i, j].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        for (i = i_LastRowMoveInput + 1, j = i_LastColumnMoveInput - 1; i <= i_LastRowMoveInput + 3 && j >= i_LastColumnMoveInput - 3; i++, j--)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i >= m_Board.GetLength(0) || j < 0)
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i, j].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        return isFourInARow;
    }

    private bool checkForLeftDiagonalWin(int i_LastRowMoveInput, int i_LastColumnMoveInput, Color i_GamePieceColor)
    {
        bool isFourInARow = false;
        int neighborsCounter = 1;
        int i = 0;
        int j = 0;

        for (i = i_LastRowMoveInput - 1, j = i_LastColumnMoveInput - 1; i >= i_LastRowMoveInput - 3 && j >= i_LastColumnMoveInput - 3; i--, j--)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i < 0 || j < 0)
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i, j].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        for (i = i_LastRowMoveInput + 1, j = i_LastColumnMoveInput + 1; i <= i_LastRowMoveInput + 3 && j <= i_LastColumnMoveInput + 3; i++, j++)
        {
            SpriteRenderer gamePieceSpriteRenderer = null;

            if (i >= m_Board.GetLength(0) || j >= m_Board.GetLength(1))
            {
                break;
            }

            gamePieceSpriteRenderer = m_Board[i, j].GetComponent<SpriteRenderer>();

            if (gamePieceSpriteRenderer.color == Color.white || gamePieceSpriteRenderer.color != i_GamePieceColor)
            {
                break;
            }

            neighborsCounter++;

            if (neighborsCounter == 4)
            {
                isFourInARow = true;
                break;
            }
        }

        return isFourInARow;
    }
}