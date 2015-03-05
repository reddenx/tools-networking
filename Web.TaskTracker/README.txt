a task tracking web app

meant to be an advanced todo list, with subtasks and a completion record

Main usabilty features:
 - touch friendly UI, for use by tablets and phones
 - responsive interface with reactive UI
 - simple clean 1 page app


TASK NAME[edit]
[active][cancelled][complete]___INPUT_________[save]

needs global input disable while ajax is processing(could be local per vm item)

SqlCe

schema notes

Task:
 - AccountId
 - TaskId
 - ParentTaskId
 - Name
 - TaskStatusId
 - DateCreated
 - DateCompleted

TaskStatus:
 - TaskStatusId
 - Name

Account
 - AccountId
 - Username