using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorSclipt : MonoBehaviour
{
    //�߰��� �� �������� �����ϴ� �迭
    public GameObject[] availableRooms;
    //���� ���ӿ� �߰��� �� ������Ʈ ����Ʈ
    public List<GameObject> currentRooms;

    //ȭ���� ���� ũ��(���� : ����)
    float screenWidthPoints;
    //�ٴ� ������Ʈ �̸�, const : ���
    const string floor = "Floor";

    //�߰��� ������Ʈ �������� �����ϴ� �迭
    public GameObject[] availableObjects;
    // ���� ���ӿ� ������ ����, ������ ������Ʈ ����Ʈ
    public List<GameObject> objects;
    // ������Ʈ�� �ּ� ����
    public float objectMinDistance = 5f;
    // ������Ʈ�� �ִ� ����
    public float objectMaxDistance = 10f;
    // ������Ʈ y�� ��ġ �ּҰ�
    public float objectMinY = -0.8f;
    // ������Ʈ y�� ��ġ �ִ밪
    public float objectMaxY = 0.8f;
    // ������Ʈ �ּ� ȸ����
    public float objectMinRotation = -40f;
    // ������Ʈ �ִ� ȸ����
    public float objectMaxRotation = 40f;

    bool fever = false;


    private void Start()
    {
        // ī�޶��� ������ ���� 2��� ���� ���� ũ�� ���
        float height = 2.0f * Camera.main.orthographicSize; //����Ƽ �󿡼� 3.2�� �����Ǿ������Ƿ� height : 6.4
        // ���� ũ�⸦ ���� �� ȭ�� ������ ���ؼ� ���� ũ�⸦ ���
        screenWidthPoints = height * Camera.main.aspect;
    }
    private void Update()
    {
        GenerateRoomIfRequired();
        GenerateObjectsRequired();
        StartCoroutine(Fever());
    }

    private void AddRoom(float farthestRooomEndX)
    {
        // �� �����յ� �� �ϳ��� �������� ����
        int randomRoomIndex = Random.Range(0, availableRooms.Length);
        // ���õ� �� ������Ʈ�� �߰�
        GameObject room = Instantiate(availableRooms[randomRoomIndex]);
        // ���� ���� ũ��
        float roomwidth = room.transform.Find(floor).localScale.x;
        // ���� �߾� ��ġ
        float roomCenter = farthestRooomEndX + roomwidth / 2;
        // ���� ���� ��ġ�� ������Ʈ�� ��ġ��Ŵ
        room.transform.position = new Vector3(roomCenter, 0, 0);
        // �߰��� ���� ���� �߰��� �� ����Ʈ�� �߰�
        currentRooms.Add(room);

    }

    private void GenerateRoomIfRequired()
    {
        // ������ ���� ����� �����ϴ� ����Ʈ
        List<GameObject> roomsToRemove = new List<GameObject>();
        // ���� �����ӿ� ���� �������� ����
        bool addRooms = true;
        // ���콺 ������Ʈ�� x�� ��ġ
        float playerX = transform.position.x;
        // ������ ���� ���� ��ġ�� ����
        float removeRoomX = playerX - screenWidthPoints;
        // �߰��� ���� ���� ��ġ�� ����
        float addRoomX = playerX + screenWidthPoints;
        // ���� �����ʿ� ��ġ�� ���� ������ �� ��ġ
        float farthestRoomEndX = 0f;

        // ���� �߰��� ���� �ϳ��� ó��
        foreach (var room in currentRooms)
        {
            // room ������Ʈ�� �ٴڿ�����Ʈ�� ã�� ����ũ�⸦ ������
            float roomWidth = room.transform.Find(floor).localScale.x;
            // ���� �߰���ġ���� ���� ũ���� ������ �� ���� �� ��ġ�� ���
            float roomStartX = room.transform.position.x - roomWidth / 2;
            // ���� ���� �� ��ġ���� ���� ũ�⸦ ���� ������ �� ��ġ�� ���

            //���� ���� �� ��ġ�� �� �߰� ���� ��ġ���� �����ʿ� ������ �� �߰�x
            float roomEndX = roomStartX + roomWidth;
            if (roomStartX > addRoomX)
                addRooms = false;
            //�� ���� ������ġ���� ���ʿ� �����ϴ� ���� ������ ����� ��Ͽ� �߰�
            if (roomEndX < removeRoomX)
                roomsToRemove.Add(room);

            //���� ������ ���� ������ �� ��ġ�� �ִ밪 �޼ҵ带 �̿��Ͽ� ����
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
                
        }

        //������ �� ������Ʈ�� �ϳ��� �����ϸ鼭
        foreach (var room in roomsToRemove)
        {
            //����Ʈ���� ����
            currentRooms.Remove(room);
            //������Ʈ ����
            Destroy(room);
        }

        //���� �߰��ؾ��Ѵٸ� ���� �߰�
        if (addRooms)
            AddRoom(farthestRoomEndX);
    }

    private void AddObject(float lastObjectX)
    {
        // �߰��� ������Ʈ�� �ε����� �������� ���ϱ�
        int randomIndex = 0;

        if (fever)
            randomIndex = Random.Range(0, availableObjects.Length - 1);
        else
            randomIndex = Random.Range(0, availableObjects.Length);

        // �������� ���� �ε��� ��ȣ�� ������Ʈ�� ����
        GameObject obj = Instantiate(availableObjects[randomIndex]);
        // ���ο� ������Ʈ�� X�� ��ġ�� ���
        float objectPositionX =
            lastObjectX + Random.Range(objectMinDistance, objectMaxDistance);
        // ���ο� ������Ʈ�� Y�� ��ġ�� ���
        float randomY = Random.Range(objectMinY, objectMaxY);

        // ���� ��ġ���� ������Ʈ�� ��ġ�� ����
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        // �������� ȸ���� ���
        float rotation = Random.Range(objectMinRotation, objectMaxRotation);

        // ���� ȸ�� ���� �ݿ��Ͽ� ���ʹϾ����� ȸ���� ����, ���ʹϾ� : ����Ƽ�� ó���ϴ� ��ǥ��
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        // ������Ʈ ����Ʈ�� �߰�
        objects.Add(obj);
    }

    private void GenerateObjectsRequired()
    {
        // �÷��̾��� X�� ��ġ
        float playerX = transform.position.x;
        // ������ ������Ʈ�� ���� ��ġ
        float removeObjectX = playerX - screenWidthPoints;
        // �߰��� ������Ʈ�� ���� ��ġ
        float addObjextX = playerX + screenWidthPoints;
        // ���� �����ʿ� ��ġ�� ������Ʈ�� x�� ��ġ
        float farthestObjectX = 0;

        // ������ ������Ʈ ����Ʈ
        List<GameObject> objectsToRemove = new List<GameObject>();

        // ���� �߰��Ǿ� �ִ� ������Ʈ���� �ϳ��� ó��
        foreach (var obj in objects)
        {
            // ������Ʈ�� X�� ��ġ
            float objX = obj.transform.position.x;
            // �ִ밪 �񱳷� ���� �����ʿ� ��ġ�� ������Ʈ�� ��ġ�� ����
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            // ������Ʈ ��ġ�� ���� ���� ��ġ���� �����̸� ������Ʈ ���� ����Ʈ�� �߰�
            if (objX < removeObjectX)
                objectsToRemove.Add(obj);
        }

        // ���� ����Ʈ�� �߰��� ������Ʈ�� ��� ����
        foreach (var obj in objectsToRemove)
        {
            // ����Ʈ���� ����
            objects.Remove(obj);
            // ������Ʈ ����
            Destroy(obj);
        }

        // ���� �����ʿ� ��ġ�� ������Ʈ�� �߰� ���� ��ġ���� �۴ٸ� ������Ʈ�� �߰�
        if (farthestObjectX < addObjextX)
            AddObject(farthestObjectX);
    }
    IEnumerator Fever()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            fever = true;
            yield return new WaitForSeconds(10f);
            fever = false;
        }
    }
}
