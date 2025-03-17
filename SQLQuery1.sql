SELECT 
    t.Id AS TestId, 
    t.Title AS TestTitle,
    q.Id AS QuestionId, 
    q.Question_Text AS QuestionText, 
    q.Image AS QuestionImage,
    a.Id AS AnswerId, 
    a.Answer_Text AS AnswerText, 
    a.Is_Correct AS IsCorrect
FROM Tests t
LEFT JOIN Questions q ON t.Id = q.Test_Id
LEFT JOIN Answers a ON q.Id = a.Question_Id
ORDER BY t.Id, q.Id, a.Id;
