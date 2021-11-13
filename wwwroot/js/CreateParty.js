function addField() {
    var addedFieldCount = $("[id^=performer-name]").length;
    if (addedFieldCount == 3) {
        alert('No more performers allowed');
        return;
    }
    $('#performersFieldContainer').append("<input id=performer-name-" + addedFieldCount + " onchange=getPerformerIdByName(this) />");
}
function postToFacebook() {
    var facebookMessage = $('#facebookMessageInput').val();
    const FacebookPageId = "100168142240956";
    const FacebookPageToken = "EAA3O6dvVFWYBAPHMDYirE5bZB7BsPuNNpofIFNtXovEK5asgrBljgOWxlytHL5ZBRjjXxjZBO0rtlRrUBQSFoLXlPczHNHP7AiZAq2K9ZBogmgXh2uZBnedScTSTTZB9g6l366ow2dCtJn5J9y9mdScuNJHTmwxfAHQURpazts73rHPjbLwBo7ZBMKMZCh0vS1JEqX2X5sPZBZCEAZDZD";
    const FacebookApi = "https://graph.facebook.com/";
    const postReqUrl = FacebookApi + FacebookPageId + "/feed?message=" + facebookMessage + "&access_token=" + FacebookPageToken;
    if (facebookMessage) {
        $.ajax({
            url: postReqUrl,
            type: "POST",
            success: function (data, textStatus, jqXHR) {},
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + " " + errorThrown);
            }
        });
    }
}
function getPerformerIdByName(item) {
    var queryParams = item.value;
    if (queryParams) {
        $.ajax({
            url: 'GetArtistIdBySearchParams/',
            type: "GET",
            data: {
                queryParams: queryParams,
            },
            success: function (result) {
                if (result === 'NO_RESULT') {
                    item.value = ""
                    alert("couldn't find artist");
                }
                else {
                    var inputElement = document.createElement("input")
                    inputElement.type = "hidden";
                    inputElement.id = "performer-id-" + item.id.split("-")[2];
                    inputElement.name = "performersId";
                    inputElement.value = result;
                    $('#performersFieldContainer').append(inputElement);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + " " + errorThrown);
            }
        });
    } else if (queryParams.length === 0) {
        var performerIdInputElementId = "#performer-id-" + item.id.split("-")[2];
        if ($(performerIdInputElementId).length) {
            console.log(performerIdInputElementId);
            $(performerIdInputElementId).remove();
        }
    }
}
