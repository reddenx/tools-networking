var TaskViewModel = function (parentItem) {
    var self = this;

    //data
    self.TaskId = ko.observable();
    self.Parent = parentItem;
    self.Name = ko.observable();
    self.Description = ko.observable();
    self.CurrentStatus = ko.observable();
    self.Children = ko.observableArray([]);

    self.EditModel = {
        EditName: ko.observable(),
        EditDescription: ko.observable(),
        EditStatus: ko.observable()
    }

    //UI states
    self.IsEditing = ko.observable(false);
    self.IsBusy = ko.observable(false);
    self.ShowChildren = ko.observable(false);
    self.HasDescription = ko.computed(function () {
        return self.Description() && self.Description().length > 0;
    });
    self.ShowDescription = ko.observable(false);
    self.ShowingCompleteChildren = ko.observable(false);

    //methods
    self.StartEditing = function () {
        self.IsEditing(true);
    }
    self.StopEditing = function () {
        self.IsEditing(false);

        self.EditModel.EditName(self.Name());
        self.EditModel.EditDescription(self.Description());
        self.EditModel.EditStatus(self.CurrentStatus());
    }
    self.AddNewChild = function () {
        var child = new TaskViewModel(self);
        child.IsEditing(true);

        self.ShowChildren(true);
        self.Children.push(child);
    }
    self.Save = function () {
        self.IsBusy(true);

        $.ajax({
            url: Urls.Ajax.SaveTask,
            type: 'POST',
            dataType: 'json',
            data:
                {
                    TaskId: self.TaskId(),
                    ParentTaskId: self.Parent ? self.Parent.TaskId() : null,
                    TaskName: self.EditModel.EditName(),
                    Description: self.EditModel.EditDescription(),
                    CurrentStatus: self.EditModel.EditStatus(),
                },
            success: function (response) {
                if (response.Success) {
                    self.AssignModelValues(response.Data);
                    self.StopEditing();

                    if (!self.HasDescription()) {
                        self.ShowDescription(false);
                    }
                }
                else {
                    alert('unable to save data');
                }
            },
            error: function (errorData) {
                alert('unable to save data');
            },
            complete: function () {
                self.IsBusy(false);
            }
        });
    }
    self.ShowCompletedChildren = function () {
        self.ShowingCompleteChildren(true);

        $.ajax({
            url: Urls.Ajax.GetAllChildrenForTask,
            type: 'POST',
            data:
            {
                taskId: self.TaskId(),
            },
            success: function (response) {
                if (response.Success) {
                    self.HandleCompletedChildrenResponse(response.Data);
                }
                else {
                    self.ShowingCompleteChildren(false);
                }
            },
            error: function () {
                self.ShowingCompleteChildren(false);
            }
        });
    }
    self.HandleCompletedChildrenResponse = function (childrenModels) {
        $.each(childrenModels, function (index, item) {
            var match = ko.utils.arrayFirst(self.Children(), function (childItem) {
                return childItem.TaskId() === item.Item.TaskId;
            });

            if (!match) {
                var child = new TaskViewModel(self);
                child.BuildModelFromServerObject(item);
                self.Children.push(child);
            }
        });
    }
    self.HideCompletedChildren = function () {

        self.Children.remove(function (item) {
            return item.CurrentStatus() === 2; //TODO-SM hardcoded values
        });

        self.ShowingCompleteChildren(false);
    }
    self.ToggleChildren = function () {
        self.ShowChildren(!self.ShowChildren());
    }

    self.BuildModelFromServerObject = function (serverModel) {

        self.AssignModelValues(serverModel.Item);

        $.each(serverModel.Children, function (index, item) {
            var taskModel = new TaskViewModel();
            taskModel.BuildModelFromServerObject(item);
            self.Children.push(taskModel);
        });

        self.StopEditing();
    }

    self.AssignModelValues = function (serverItem) {
        self.TaskId(serverItem.TaskId);
        self.Name(serverItem.Name);
        self.Description(serverItem.Description);
        self.CurrentStatus(serverItem.CurrentStatus);
    }

    return self;
}