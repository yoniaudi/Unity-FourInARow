using UnityEngine;

public class ComputerPlayerAIModel : MonoBehaviour
{
    [SerializeField] private Color m_WhiteGamePiece = Color.white;
    [SerializeField] private Color m_RedGamePiece = Color.red;
    [SerializeField] private Color m_GreenGamePiece = Color.green;
    public const string k_Name = "Computer AI";
    public int RowMoveInput { get; set; } = 0;
    public int ColumnMoveInput { get; set; } = 0;
    public GameObject[,] Board { get; set; } = null;

    /*public ComputerPlayerAIModel(BoardGameModel i_BoardGame)
    {
        m_Board = i_BoardGame.GetBoardGame();
    }*/

    public int GetBestMove()
    {
        int bestMove = -1;
        int bestScore = int.MinValue;

        for (int column = 0; column < Board.GetLength(1); column++)
        {
            int row = 0;
            int score = 0;
            SpriteRenderer gamePieceSpriteRenderer = Board[row, column].GetComponent<SpriteRenderer>();

            if (isColumnFull(column))
            {
                continue;
            }

            row = getEmptyRow(column);
            gamePieceSpriteRenderer.color = m_GreenGamePiece;
            score = evaluateBoard(m_GreenGamePiece);
            gamePieceSpriteRenderer.color = m_WhiteGamePiece;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = column;
                RowMoveInput = row;
                ColumnMoveInput = bestMove;
            }
        }

        return bestMove;
    }

    private bool isColumnFull(int i_Column)
    {
        return Board[0, i_Column].GetComponent<SpriteRenderer>().color != m_WhiteGamePiece;
    }

    private int getEmptyRow(int i_Column)
    {
        int row = -1;

        for (int i = Board.GetLength(0) - 1; i >= 0; i--)
        {
            if (Board[i, i_Column].GetComponent<SpriteRenderer>().color == m_WhiteGamePiece)
            {
                row = i;
                break;
            }
        }

        return row;
    }

    private int evaluateBoard(Color i_ComputerPlayerGamePiece)
    {
        Color opponentGamePiece = m_RedGamePiece;
        int score = 0;
        bool isOpponentTurn = true;

        score += evaluatePotentialWins(i_ComputerPlayerGamePiece, !isOpponentTurn);
        score -= evaluatePotentialWins(opponentGamePiece, isOpponentTurn);

        return score;
    }

    private int evaluatePotentialWins(Color i_Player, bool i_IsOpponent)
    {
        int score = 0;

        score += evaluateDirection(i_Player, 0, 1, i_IsOpponent);
        score += evaluateDirection(i_Player, 0, -1, i_IsOpponent);
        score += evaluateDirection(i_Player, 1, 0, i_IsOpponent);
        score += evaluateDirection(i_Player, 1, 1, i_IsOpponent);
        score += evaluateDirection(i_Player, -1, -1, i_IsOpponent);
        score += evaluateDirection(i_Player, 1, -1, i_IsOpponent);
        score += evaluateDirection(i_Player, -1, 1, i_IsOpponent);

        return score;
    }

    private int evaluateDirection(Color i_Player, int i_RowDirection, int i_ColDirection, bool i_IsOpponent)
    {
        int score = 0;

        for (int row = 0; row < Board.GetLength(0); row++)
        {
            for (int col = 0; col < Board.GetLength(1); col++)
            {
                int count = 0;
                int emptyCount = 0;

                for (int i = 0; i < 4; i++)
                {
                    int newRow = row + i * i_RowDirection;
                    int newCol = col + i * i_ColDirection;

                    if (newRow >= 0 && newRow < Board.GetLength(0) && newCol >= 0 && newCol < Board.GetLength(1))
                    {
                        if (Board[newRow, newCol].GetComponent<SpriteRenderer>().color == i_Player)
                        {
                            count++;
                        }
                        else if (Board[newRow, newCol] == null)
                        {
                            emptyCount++;
                        }
                        else
                        {
                            count = 0;
                            emptyCount = 0;
                            break;
                        }
                    }
                    else
                    {
                        count = 0;
                        emptyCount = 0;
                        break;
                    }
                }

                if (i_IsOpponent == false)
                {
                    score += countToScore(count, emptyCount);
                }
                else
                {
                    score += countToScoreForOpponent(count, emptyCount);
                }
            }
        }

        return score;
    }

    private int countToScore(int i_Count, int i_EmptyCount)
    {
        int score = 0;

        switch (i_Count)
        {
            case 1:
                score = i_EmptyCount == 3 ? 1 : 0;
                break;
            case 2:
                score = i_EmptyCount == 2 ? 10 : 0;
                break;
            case 3:
                score = i_EmptyCount == 1 ? 100 : 0;
                break;
            case 4:
                score = 500;
                break;
            default:
                score = 0;
                break;
        }

        return score;
    }

    private int countToScoreForOpponent(int i_Count, int i_EmptyCount)
    {
        int score = 0;

        switch (i_Count)
        {
            case 1:
                score = i_EmptyCount == 3 ? 1 : 0;
                break;
            case 2:
                score = i_EmptyCount == 2 ? 10 : 0;
                break;
            case 3:
                score = i_EmptyCount == 1 ? 500 : 0;
                break;
            case 4:
                score = 1000;
                break;
            default:
                score = 0;
                break;
        }

        return score;
    }
}
