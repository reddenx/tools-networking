var SummaryViewModel = function () {
    var self = this;

    self.IsBusy = ko.observable(true);
    self.TaskTreeTrunk = ko.observableArray([]);
    self.Error = ko.observable();

    self.GetTree = function (startingTaskId) {

        if (startingTaskId) {
            $.ajax({
                url: Urls.Ajax.GetAllChildrenForTask,
                type: 'POST',
                data: { 'taskId': startingTaskId },
                success: function (response) {
                    if (response.Success) {
                        self.HandleTreeData(response.Data);
                    } else {
                        self.Error(response.ErrorMessage);
                    }
                },
                error: function (errorInfo) { },
                complete: function () {
                    self.IsBusy(false);
                }
            });
        }
        else {
            $.ajax({
                url: Urls.Ajax.GetTreeItems,
                type: 'POST',
                dataType: 'json',
                data: { 'accountId': 1 },//TODO-SM Hardcoded accountId here
                success: function (response) {
                    if (response.Success) {
                        self.HandleTreeData(response.Data);
                    }
                    else {
                        self.Error(response.ErrorMessage);
                    }
                },
                error: function (errorInfo) {
                    self.Error('Unable to fetch task information');
                },
                complete: function () {
                    self.IsBusy(false);
                }
            });
        }
    }

    self.HandleTreeData = function (taskItems) {

        $.each(taskItems, function (index, item) {
            var taskModel = new TaskViewModel();
            taskModel.BuildModelFromServerObject(item);
            self.TaskTreeTrunk.push(taskModel);
        });

    }

    return self;
}